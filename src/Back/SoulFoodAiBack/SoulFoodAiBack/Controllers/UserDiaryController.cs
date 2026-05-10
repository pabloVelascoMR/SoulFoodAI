using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using static SoulFoodAiBack.Dtos.WeeklyReportDto;

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
                UserData? userData = await _context.UserDatas
                    .Include(u => u.FoodPlan)
                    .Include(u => u.UserIntolerances)
                    .FirstOrDefaultAsync(u => u.IdUser == dto.IdUser);

                UserFoodPlanWeek? activePlan = await _context.UserFoodPlansWeek
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
                    double oldProtein = Math.Round(userData.FoodPlan.ProteinPercent * 100);
                    double oldCarbs = Math.Round(userData.FoodPlan.CarbPercent * 100);
                    double oldFat = Math.Round(userData.FoodPlan.FatPercent * 100);

                   
                    string prompt = $@"
            Eres un nutricionista clínico experto. Analiza este reporte semanal y propón ajustes matemáticos y estratégicos precisos:

            DATOS ACTUALES (Semana pasada):
            - Calorías diarias: {oldDailyKcal} kcal
            - Reparto de Macros: Proteína {oldProtein}%, Carbohidratos {oldCarbs}%, Grasas {oldFat}%

            REPORTE DEL USUARIO:
            - Saciedad (1-10): {dto.HungerLevel} (1=hambre atroz, 10=empachado)
            - Energía (1-10): {dto.EnergyLevel} (1=agotado, 10=pico de energía)
            - Sueño (1-10): {dto.SleepQuality} (1=pésimo, 10=perfecto)
            - Adherencia: {dto.DietAdherence}/10
            - Peso actual: {weightToUse} kg
            - Observaciones: ""{dto.Description ?? "Sin comentarios adicionales"}""

            INSTRUCCIONES CRÍTICAS PARA TU ANÁLISIS:
            1. Calcula las calorías y macros (en porcentajes sobre 1, ej: 0.30) para la PRÓXIMA SEMANA.
            2. En el campo 'AnalysisMessage', DEBES redactar un informe detallado y empático dirigido al usuario.
            3. En tu informe, DEBES mencionar explícitamente los cambios numéricos (ej. de {oldDailyKcal} kcal a X kcal, de {oldCarbs}% CH a Y% CH).
            4. DEBES justificar el POR QUÉ de estos cambios basándote DIRECTAMENTE en sus puntuaciones y especialmente en sus Observaciones.
            5. Usa decimales con punto (ej. 0.40) para los SuggestedPercentages en el JSON.
            6. Si el usuaro especifica exactamente en el campo observaciones las kilocaliorias o porcentajes de macros o gramos de proteinas o gramos de hidratos de carbono o gramos de grassas que quiere tener, tendra mas importancia que cualquier otro ajuste y el nuevo plan tendra las especificaciones que dicte 

            Devuelve ESTRICTAMENTE este JSON sin markdown:
            {{
                ""AnalysisMessage"": ""(Ejemplo: 'He leído en tus notas que has tenido mucha ansiedad por la tarde y veo que tu energía fue baja (4/10). Para solucionarlo, he ajustado tus calorías pasando de 2000 kcal a 2150 kcal. Además, he modificado tus macros: bajamos las grasas del 30% al 25% y subimos los carbohidratos del 40% al 45% para darte ese extra de energía constante que necesitas...')"",
                ""SuggestedKcal"": 2150,
                ""SuggestedProteinPercentage"": 0.30,
                ""SuggestedCarbsPercentage"": 0.45,
                ""SuggestedFatPercentage"": 0.25
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

                    FoodPlanAdjustmentAIDto? aiData = JsonSerializer.Deserialize<FoodPlanAdjustmentAIDto>(aiResponseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

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
                    UserDiary? diaryEntry = new UserDiary
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

                double dailyProteinGrams = (finalDailyKcal * proteinPercent) / 4.0;
                double dailyCarbsGrams = (finalDailyKcal * carbPercent) / 4.0;
                double dailyFatGrams = (finalDailyKcal * fatPercent) / 9.0;

                DateTime today = DateTime.Today;
                UserFoodPlanWeek newWeek = new UserFoodPlanWeek
                {
                    IdUser = dto.IdUser,
                    IdFoodPlan = userData.IdFoodPlan,
                    IdGoal = userData.IdGoal,
                    StartDate = today,
                    EndDate = today.AddDays(6),
                    MealsPerDay = userData.MealsPerDay,
                    TotalWeeklyKcal = finalDailyKcal * 7,
                    TotalWeeklyProtein = dailyProteinGrams * 7,
                    TotalWeeklyCarbs = dailyCarbsGrams * 7,
                    TotalWeeklyFat = dailyFatGrams * 7,
                    IsActive = true,
                    UserFoodPlanWeekIntolerances = new List<UserFoodPlanWeekIntolerance>()
                };

                if (userData.UserIntolerances != null && userData.UserIntolerances.Any())
                {
                    foreach (UserIntolerance? intol in userData.UserIntolerances)
                    {
                        newWeek.UserFoodPlanWeekIntolerances.Add(new UserFoodPlanWeekIntolerance { IdIntolerance = intol.IdIntolerance });
                    }
                }

                await _context.UserFoodPlansWeek.AddAsync(newWeek);
                await _context.SaveChangesAsync();

                for (int i = 0; i < 7; i++)
                {
                    await _context.UserFoodPlansDaily.AddAsync(new UserFoodPlanDaily
                    {
                        IdUser = dto.IdUser,
                        IdUserFoodPlanWeek = newWeek.IdUserFoodPlanWeek,
                        IdFoodPlan = userData.IdFoodPlan,
                        TargetKcal = finalDailyKcal,
                        TargetProtein = dailyProteinGrams,
                        TargetCarbs = dailyCarbsGrams,
                        TargetFat = dailyFatGrams,
                        RealKcal = 0,
                        RealProtein = 0,
                        RealCarbs = 0,
                        RealFat = 0,
                        IsActive = true,
                        CreationDate = today.AddDays(i)
                    });
                }
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    IdNewUserFoodPlanWeek = newWeek.IdUserFoodPlanWeek,
                    AiAnalysis = aiMessage
                });

            }
            catch (TaskCanceledException)
            {
                return StatusCode(500, new { message = "Los servidores de IA están muy saturados y han tardado demasiado en responder. Por favor, vuelve a intentarlo en un momento." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno del servidor: {ex.Message}" });
            }
        }
    }
}