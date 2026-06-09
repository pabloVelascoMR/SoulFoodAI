using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using System.Text;
using System.Text.Json;
using static SoulFoodAiBack.Dtos.WeeklyReportDto;

namespace SoulFoodAiBack.Controllers
{
    /// <summary>
    /// Controlador encargado de la gestión del progreso y revisiones periódicas del usuario. 
    /// Recibe el reporte de final de semana, lo procesa, y mediante integración con Inteligencia Artificial (Gemini), 
    /// calibra los macronutrientes para el diseño del plan nutricional de la siguiente iteración semanal.
    /// </summary>
    /// <remarks>
    /// @author Pablo_Velasco_Martin
    /// @see SoulFoodAiBack.Controllers
    /// @see SoulFoodAiBack.Models
    /// @see SoulFoodAiBack.Dtos
    /// @see SoulFoodAiBack.Data
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class UserDiaryController : ControllerBase
    {
        /// <summary>
        /// Contexto inyectado de la base de datos para la persistencia de las revisiones.
        /// </summary>
        private readonly DataContext _context;

        /// <summary>
        /// Proveedor de configuración para la lectura de parámetros del entorno (ej. API Key de la IA).
        /// </summary>
        private readonly IConfiguration _config;

        /// <summary>
        /// Cliente HTTP estático instanciado para canalizar las peticiones a la API externa de Inteligencia Artificial.
        /// </summary>
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(3) };
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        /// <summary>
        /// Constructor que inyecta el contexto de datos y la interfaz de configuración de la aplicación.
        /// </summary>
        /// <param name="context">Instancia del contexto de Entity Framework.</param>
        /// <param name="config">Gestor de configuración del proyecto.</param>
        public UserDiaryController(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        private static void CalculateClassicFinalKcal(UserData userData, double weightToUse, out int finalDailyKcal, out double proteinPercent, out double carbPercent, out double fatPercent)
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
            proteinPercent = userData.FoodPlan?.ProteinPercent ?? 0;
            carbPercent = userData.FoodPlan?.CarbPercent ?? 0;
            fatPercent = userData.FoodPlan?.FatPercent ?? 0;
        }

        private async Task<(int, double, double, double, string)> CalculateAiAdjustmentAsync(WeeklyReportDto dto, UserData userData, UserFoodPlanWeek? activePlan, double weightToUse)
        {
            int oldDailyKcal = (activePlan != null) ? (activePlan.TotalWeeklyKcal / 7) : 0;
            double oldProtein = Math.Round((userData.FoodPlan?.ProteinPercent ?? 0) * 100);
            double oldCarbs = Math.Round((userData.FoodPlan?.CarbPercent ?? 0) * 100);
            double oldFat = Math.Round((userData.FoodPlan?.FatPercent ?? 0) * 100);
            string prompt = $@"
            Eres un nutricionista cl nico experto. Analiza este reporte semanal y prop n ajustes matem ticos y estrat gicos precisos:
            DATOS ACTUALES (Semana pasada):
            - Calor as diarias: {oldDailyKcal} kcal
            - Reparto de Macros: Prote na {oldProtein}%, Carbohidratos {oldCarbs}%, Grasas {oldFat}%
            REPORTE DEL USUARIO:
            - Saciedad (1-10): {dto.HungerLevel} (1=hambre atroz, 10=empachado)
            - Energ a (1-10): {dto.EnergyLevel} (1=agotado, 10=pico de energ
            - Sue o (1-10): {dto.SleepQuality} (1=p simo, 10=perfecto)
            - Adherencia: {dto.DietAdherence}/10
            - Peso actual: {weightToUse} kg
            - Observaciones: ""{dto.Description ?? "Sin comentarios adicionales"}""
            INSTRUCCIONES CR TICAS PARA TU AN LISIS:
            1. Calcula las calor as y macros (en porcentajes sobre 1, ej: 0.30) para la PR XIMA SEMANA.
            2. En el campo 'AnalysisMessage', DEBES redactar un informe detallado y emp tico dirigido al usuario.
            3. En tu informe, DEBES mencionar expl citamente los cambios num ricos (ej. de {oldDailyKcal} kcal a X kcal, de {oldCarbs}% CH a Y% CH).
            4. DEBES justificar el POR QU  de estos cambios bas ndote DIRECTAMENTE en sus puntuaciones y especialmente en sus Observaciones.
            5. Usa decimales con punto (ej. 0.40) para los SuggestedPercentages en el JSON.
            6. Si el usuaro especifica exactamente en el campo observaciones las kilocaliorias o porcentajes de macros o gramos de proteinas o gramos de hidratos de carbono o gramos de grassas que quiere tener, tendra mas importancia que cualquier otro ajuste y el nuevo plan tendra las especificaciones que dicte 
             Devuelve ESTRICTAMENTE este JSON sin markdown:
            {{
                ""AnalysisMessage"": ""(Ejemplo: 'He le do en tus notas que has tenido mucha ansiedad por la tarde y veo que tu energ a fue baja (4/10). Para solucionarlo, he ajustado tus calor as pasando de 2000 kcal a 2150 kcal. Adem s, he modificado tus macros: bajamos las grasas del 30% al 25% y subimos los carbohidratos del 40% al 45% para darte ese extra de energ a constante que necesitas...')"",
                ""SuggestedKcal"": 2150,
                ""SuggestedProteinPercentage"": 0.30,
                ""SuggestedCarbsPercentage"": 0.45,
                ""SuggestedFatPercentage"": 0.25
            }}";
            string apiKey = _config["Gemini:ApiKey"] ?? string.Empty;
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";
            var requestBody = new
            {
                contents = new[] { new { parts = new[] { new { text = prompt } } } },
                generationConfig = new { responseMimeType = "application/json", temperature = 0.7 }
            };
            var content = new StringContent(JsonSerializer.Serialize(requestBody, _jsonOptions), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(await response.Content.ReadAsStringAsync());
            }
            string responseString = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(responseString);
            string aiResponseJson = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? "";
            aiResponseJson = aiResponseJson.Replace("```json", "").Replace("```", "").Trim();
            FoodPlanAdjustmentAIDto? aiData = JsonSerializer.Deserialize<FoodPlanAdjustmentAIDto>(aiResponseJson, _jsonOptions);
            if (aiData != null)
            {
                return (aiData.SuggestedKcal, aiData.SuggestedProteinPercentage, aiData.SuggestedCarbsPercentage, aiData.SuggestedFatPercentage, aiData.AnalysisMessage ?? aiResponseJson);
            }
            return (0, 0, 0, 0, string.Empty);
        }

        private async Task DeactivateCurrentPlanAsync(UserFoodPlanWeek activePlan, WeeklyReportDto dto, string aiMessage)
        {
            UserDiary diaryEntry = new UserDiary
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
            List<UserFoodPlanDaily> activeDailies = await _context.UserFoodPlansDaily
                .Include(d => d.FoodPlanDailyRecipes)
                .Where(d => d.IdUserFoodPlanWeek == activePlan.IdUserFoodPlanWeek)
                .ToListAsync();
            foreach (var daily in activeDailies)
            {
                daily.IsActive = false;
                if (daily.FoodPlanDailyRecipes != null && daily.FoodPlanDailyRecipes.Count > 0)
                {
                    foreach (var recipe in daily.FoodPlanDailyRecipes)
                    {
                        recipe.IsActive = false;
                    }
                }
            }
        }

        private void UpdateUserMeasures(UserData userData, WeeklyReportDto dto)
        {
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
        }

        private async Task<UserFoodPlanWeek> CreateNewWeeklyPlanAsync(WeeklyReportDto dto, UserData userData, int finalDailyKcal, double proteinPercent, double carbPercent, double fatPercent)
        {
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
                IsVisibleInHistory = true,
                UserFoodPlanWeekIntolerances = new List<UserFoodPlanWeekIntolerance>()
            };
            if (userData.UserIntolerances != null && userData.UserIntolerances.Count > 0)
            {
                foreach (UserIntolerance? intol in userData.UserIntolerances)
                {
                    if (intol != null)
                    {
                        newWeek.UserFoodPlanWeekIntolerances.Add(new UserFoodPlanWeekIntolerance { IdIntolerance = intol.IdIntolerance });
                    }
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
            return newWeek;
        }

        /// <summary>
        /// Procesa el informe semanal del usuario (incluyendo variables cualitativas como hambre, nivel de energía y adherencia)
        /// y determina, empleando el motor de IA, los ajustes metabólicos precisos para estructurar la nueva planificación semanal.
        /// </summary>
        /// <param name="dto">Objeto de transferencia de datos con las métricas y el feedback recopilado del usuario.</param>
        /// <returns>Objeto estructurado con el identificador de la nueva semana y un análisis descriptivo autogenerado por la IA.</returns>
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
                double proteinPercent = 0, carbPercent = 0, fatPercent = 0;
                string aiMessage = string.Empty;
                double weightToUse = (dto.NewWeight.HasValue && dto.NewWeight.Value > 0) ? dto.NewWeight.Value : userData.Weight;
                if (!dto.UseAiAdjustment)
                {
                    CalculateClassicFinalKcal(userData, weightToUse, out finalDailyKcal, out proteinPercent, out carbPercent, out fatPercent);
                }
                else
                {
                    var (aiKcal, aiProt, aiCarb, aiFat, aiMsg) = await CalculateAiAdjustmentAsync(dto, userData, activePlan, weightToUse);
                    finalDailyKcal = aiKcal;
                    proteinPercent = aiProt;
                    carbPercent = aiCarb;
                    fatPercent = aiFat;
                    aiMessage = aiMsg;
                }
                if (activePlan != null)
                {
                    await DeactivateCurrentPlanAsync(activePlan, dto, aiMessage);
                }
                UpdateUserMeasures(userData, dto);
                var newWeek = await CreateNewWeeklyPlanAsync(dto, userData, finalDailyKcal, proteinPercent, carbPercent, fatPercent);
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