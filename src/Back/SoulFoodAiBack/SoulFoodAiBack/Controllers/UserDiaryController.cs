using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using System.Text;
using System.Text.Json;

namespace SoulFoodAiBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserDiaryController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(3) };

        public UserDiaryController(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost]
        [Route("SubmitReportAndCreatePlan")]
        public async Task<IActionResult> SubmitReportAndCreatePlan([FromBody] WeeklyReportDto dto)
        {
            try
            {
                var userData = await _context.UserDatas
                    .Include(u => u.FoodPlan)
                    .Include(u => u.UserIntolerances)
                    .FirstOrDefaultAsync(u => u.IdUser == dto.IdUser);

                var activePlan = await _context.UserFoodPlansWeek
                    .FirstOrDefaultAsync(p => p.IdUser == dto.IdUser && p.IsActive);

                if (userData == null)
                    return NotFound(new { message = "Usuario no encontrado." });

                int finalDailyKcal = 0;
                double proteinPercent = userData.FoodPlan.ProteinPercent;
                double carbPercent = userData.FoodPlan.CarbPercent;
                double fatPercent = userData.FoodPlan.FatPercent;
                string aiMessage = string.Empty;

                double weightToUse = (dto.NewWeight.HasValue && dto.NewWeight.Value > 0) ? dto.NewWeight.Value : userData.Weight;

                if (!dto.UseAiAdjustment)
                {
                    double bmr = userData.Gender == "Hombre"
                        ? (10 * weightToUse) + (6.25 * (userData.Height * 100)) - (5 * userData.Age) + 5
                        : (10 * weightToUse) + (6.25 * (userData.Height * 100)) - (5 * userData.Age) - 161;

                    double tdee = bmr * 1.3;
                    double dailyKcalTarget = tdee;

                    switch (userData.IdGoal)
                    {
                        case 1: dailyKcalTarget -= 400; break;
                        case 2: dailyKcalTarget += 300; break;
                        case 4: dailyKcalTarget -= 200; break;
                        case 5: dailyKcalTarget += 250; break;
                    }

                    finalDailyKcal = (int)Math.Round(dailyKcalTarget);
                    if (finalDailyKcal < 1200) finalDailyKcal = 1200;
                }
                else
                {
                    int oldDailyKcal = (activePlan != null) ? (activePlan.TotalWeeklyKcal / 7) : 0;
                    double oldProteinGrams = (activePlan != null) ? Math.Round(activePlan.TotalWeeklyProtein / 7) : 0;
                    double oldCarbsGrams = (activePlan != null) ? Math.Round(activePlan.TotalWeeklyCarbs / 7) : 0;
                    double oldFatGrams = (activePlan != null) ? Math.Round(activePlan.TotalWeeklyFat / 7) : 0;

                    string prompt = $@"
            Actúa como un Nutricionista Deportivo de Élite. Ajusta el plan de este paciente basándote en su reporte.

            DIETA ANTERIOR (Semana pasada):
            - Calorías: {oldDailyKcal} kcal/día
            - Macros: {oldProteinGrams}g Proteína, {oldCarbsGrams}g Carbohidratos, {oldFatGrams}g Grasas.

            FEEDBACK DEL PACIENTE:
            - Saciedad/Hambre (1-10): {dto.HungerLevel} 
            - Energía (1-10): {dto.EnergyLevel} 
            - Sueño (1-10): {dto.SleepQuality}/10
            - Adherencia: {dto.DietAdherence}/10
            - Peso actual: {weightToUse} kg
            - OBSERVACIONES CLAVE: ""{dto.Description ?? "Sin comentarios"}""

            INSTRUCCIONES OBLIGATORIAS:
            1. En 'AnalysisMessage', DEBES escribir un informe técnico explicando los CAMBIOS EXACTOS EN GRAMOS Y KCAL.
            2. Ejemplo de cómo debes redactar: 'Al notar en tus observaciones...'
            3. Devuelve ESTRICTAMENTE este JSON válido. No uses saltos de línea (usa \n si lo necesitas).

            {{
                ""AnalysisMessage"": ""Tu informe detallado aquí..."",
                ""suggestedKcal"": 2200,
                ""suggestedProteinPercentage"": 0.30,
                ""suggestedCarbsPercentage"": 0.45,
                ""suggestedFatPercentage"": 0.25
            }}";

                    string apiKey = _config["Gemini:ApiKey"];
                    string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

                    var requestBody = new
                    {
                        contents = new[] { new { parts = new[] { new { text = prompt } } } },
                        generationConfig = new { responseMimeType = "application/json", temperature = 0.7 }
                    };

                    var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await _httpClient.PostAsync(url, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        string errorApi = await response.Content.ReadAsStringAsync();
                        return StatusCode(500, new { message = $"Error de los servidores de Google Gemini: {errorApi}" });
                    }

                    string responseString = await response.Content.ReadAsStringAsync();
                    using JsonDocument doc = JsonDocument.Parse(responseString);
                    string aiResponseJson = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? "";
                    aiResponseJson = aiResponseJson.Replace("```json", "").Replace("```", "").Trim();

                    var aiData = JsonSerializer.Deserialize<FoodPlanAdjustmentAIDto>(aiResponseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (aiData != null)
                    {
                        finalDailyKcal = aiData.SuggestedKcal;
                        proteinPercent = aiData.SuggestedProteinPercentage;
                        carbPercent = aiData.SuggestedCarbsPercentage;
                        fatPercent = aiData.SuggestedFatPercentage;
                        aiMessage = aiData.AnalysisMessage ?? aiResponseJson;
                    }
                }

                if (activePlan != null)
                {
                    var diaryEntry = new UserDiary
                    {
                        HungerLevel = dto.HungerLevel,
                        EnergyLevel = dto.EnergyLevel,
                        SleepQuality = dto.SleepQuality,
                        DietAdherence = dto.DietAdherence,
                        Description = dto.Description,
                        AiReportResponse = aiMessage,  
                        IdUserFoodPlan = activePlan.IdUserFoodPlanWeek,
                        UserFoodPlan = activePlan,
                        CreationDate = DateTime.Now
                    };
                    await _context.UserDiaries.AddAsync(diaryEntry);
                    activePlan.IsActive = false;
                }

             
                if (dto.NewWeight.HasValue && dto.NewWeight.Value > 0) userData.Weight = dto.NewWeight.Value;
                if (dto.NewMeasures != null)
                {
                    if (dto.NewMeasures.ChestMeasure > 0) userData.ChestMeasure = dto.NewMeasures.ChestMeasure;
                    if (dto.NewMeasures.WaistMeasure > 0) userData.WaistMeasure = dto.NewMeasures.WaistMeasure;
                    if (dto.NewMeasures.HipMeasure > 0) userData.HipMeasure = dto.NewMeasures.HipMeasure;
                    if (dto.NewMeasures.LeftBicepMeasure > 0) userData.LeftBicepMeasure = dto.NewMeasures.LeftBicepMeasure;
                    if (dto.NewMeasures.RightBicepMeasure > 0) userData.RightBicepMeasure = dto.NewMeasures.RightBicepMeasure;
                    if (dto.NewMeasures.LeftCuadricepsMeasure > 0) userData.LeftCuadricepsMeasure = dto.NewMeasures.LeftCuadricepsMeasure;
                    if (dto.NewMeasures.RightCuadricepsMeasure > 0) userData.RightCuadricepsMeasure = dto.NewMeasures.RightCuadricepsMeasure;
                }

                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Cálculo realizado y base de datos actualizada.", AiAnalysis = aiMessage });
            }
            catch (TaskCanceledException)
            {
                return StatusCode(500, new { message = "Los servidores de IA están muy saturados." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno del servidor: {ex.Message}" });
            }
        }
    }
}