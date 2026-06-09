using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace SoulFoodAiBack.Controllers
{
    /// <summary>
    /// Controlador responsable de la gestión pormenorizada de los días dentro del plan semanal. 
    /// Expone operaciones para la consulta del progreso diario, la manipulación de las recetas asignadas 
    /// y el ajuste dinámico de macronutrientes apoyado en algoritmos de Inteligencia Artificial.
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
    public class UserFoodPlanDailyController : ControllerBase
    {
        /// <summary>
        /// Contexto de acceso a datos para transacciones locales con la base de datos de Entity Framework.
        /// </summary>
        private readonly DataContext _context;

        /// <summary>
        /// Cliente HTTP para la comunicación persistente y asíncrona con la API de generación de IA.
        /// </summary>
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Gestor de configuración para la lectura segura de credenciales y variables de entorno del sistema.
        /// </summary>
        private readonly IConfiguration _config;
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        /// <summary>
        /// Constructor con inyección de dependencias para proveer el contexto relacional y la configuración de entorno.
        /// </summary>
        /// <param name="context">Instancia del contexto de Entity Framework.</param>
        /// <param name="config">Interfaz de lectura de configuración general.</param>
        public UserFoodPlanDailyController(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        /// <summary>
        /// Consulta el resumen general de un día específico, proyectando los datos nutricionales reales 
        /// frente a las calorías y macronutrientes objetivo para su correcta visualización.
        /// </summary>
        /// <param name="idDailyPlan">Identificador principal del día registrado en el plan.</param>
        /// <returns>Objeto estructurado con la cabecera y el resumen porcentual del progreso del día.</returns>
        [HttpGet]
        [Route("GetDailyHeader/{idDailyPlan}")]
        public async Task<IActionResult> GetDailyHeader(int idDailyPlan)
        {
            UserFoodPlanDaily? day = await _context.UserFoodPlansDaily
                .Include(d => d.FoodPlan)
                .Include(d => d.UserFoodPlanWeek)
                .FirstOrDefaultAsync(d => d.IdUserFoodPlanDaily == idDailyPlan);
            if (day == null)
                return NotFound("No se ha encontrado el d a especificado.");
            CultureInfo culture = new CultureInfo("es-ES");
            string formattedDayName = day.CreationDate.ToString("dddd dd", culture);
            formattedDayName = char.ToUpper(formattedDayName[0]) + formattedDayName.Substring(1);
            DailyHeaderDto dto = new DailyHeaderDto
            {
                IdUserFoodPlanDaily = day.IdUserFoodPlanDaily,
                DietName = day.FoodPlan?.FoodPlanName ?? string.Empty,
                DayName = formattedDayName,
                TargetKcal = day.TargetKcal,
                RealKcal = day.RealKcal,
                TargetProtein = Math.Round(day.TargetProtein, 1),
                RealProtein = Math.Round(day.RealProtein, 1),
                TargetCarbs = Math.Round(day.TargetCarbs, 1),
                RealCarbs = Math.Round(day.RealCarbs, 1),
                TargetFat = Math.Round(day.TargetFat, 1),
                RealFat = Math.Round(day.RealFat, 1),
                MealsPerDay = day.UserFoodPlanWeek?.MealsPerDay ?? 0
            };
            return Ok(dto);
        }

        /// <summary>
        /// Efectúa la actualización y reemplazo de las recetas programadas para una jornada concreta,
        /// recalculando y consolidando de manera automática el sumatorio de sus métricas nutricionales.
        /// </summary>
        /// <param name="dto">Objeto de transferencia que mapea el identificador del día con la lista de recetas.</param>
        /// <returns>Devuelve un código 200 (OK) en caso de que la sincronización transaccional se ejecute con éxito.</returns>
        [HttpPost]
        [Route("UpdateDailyRecipes")]
        public async Task<IActionResult> UpdateDailyRecipes([FromBody] UpdateDailyRecipesDto dto)
        {
            var targetDay = await _context.UserFoodPlansDaily
                .FirstOrDefaultAsync(d => d.IdUserFoodPlanDaily == dto.IdUserFoodPlanDaily);
            if (targetDay == null)
                return NotFound("No se encontr  el d a especificado.");
            var existingRecipes = await _context.FoodPlanDailyRecipes
                .Where(dr => dr.IdUserFoodPlanDaily == dto.IdUserFoodPlanDaily)
                .ToListAsync();
            if (existingRecipes.Count > 0)
            {
                _context.FoodPlanDailyRecipes.RemoveRange(existingRecipes);
            }
            double totalRealKcal = 0;
            double totalRealProtein = 0;
            double totalRealCarbs = 0;
            double totalRealFat = 0;
            foreach (int recipeId in dto.RecipeIds)
            {
                var recipeInfo = await _context.Recipes.FindAsync(recipeId);
                if (recipeInfo != null)
                {
                    await _context.FoodPlanDailyRecipes.AddAsync(new FoodPlanDailyRecipe
                    {
                        IdUserFoodPlanDaily = dto.IdUserFoodPlanDaily,
                        IdRecipe = recipeId,
                        IdUser = targetDay.IdUser,
                        IsActive = true
                    });
                    totalRealKcal += recipeInfo.TotalKcal;
                    totalRealProtein += recipeInfo.Protein;
                    totalRealCarbs += recipeInfo.Carbs;
                    totalRealFat += recipeInfo.Fat;
                }
            }
            targetDay.RealKcal = (int)totalRealKcal;
            targetDay.RealProtein = Math.Round(totalRealProtein, 1);
            targetDay.RealCarbs = Math.Round(totalRealCarbs, 1);
            targetDay.RealFat = Math.Round(totalRealFat, 1);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "D a actualizado y guardado correctamente." });
        }

        private static string BuildAdjustmentPrompt(UserFoodPlanDaily dailyPlan)
        {
            var promptBuilder = new StringBuilder();
            promptBuilder.AppendLine("Eres un nutricionista experto. Tu objetivo es ajustar los gramos de los ingredientes de las recetas de un d a para cuadrar los macros exactos.");
            promptBuilder.AppendLine($"\nOBJETIVOS DEL D A:");
            promptBuilder.AppendLine($"- Kcal: {dailyPlan.TargetKcal}");
            promptBuilder.AppendLine($"- Prote na: {dailyPlan.TargetProtein}g");
            promptBuilder.AppendLine($"- Carbohidratos: {dailyPlan.TargetCarbs}g");
            promptBuilder.AppendLine($"- Grasas: {dailyPlan.TargetFat}g\n");
            promptBuilder.AppendLine("RECETAS ACTUALES Y SUS INGREDIENTES (Macros por 100g):");
            foreach (FoodPlanDailyRecipe? fpdr in dailyPlan.FoodPlanDailyRecipes)
            {
                if (fpdr?.Recipe == null) continue;
                var r = fpdr.Recipe;
                promptBuilder.AppendLine($"Receta [ID_FPDR: {fpdr.IdFoodPlanDailyRecipe}]: {r.RecipeName}");
                foreach (var ing in r.RecipeUserIngredients)
                {
                    if (ing?.Ingredient != null)
                    {
                        promptBuilder.AppendLine($"  - Ingrediente [ID_ING: {ing.IdIngredient}]: {ing.Ingredient.Name} | Prot: {ing.Ingredient.Protein}g | Carbs: {ing.Ingredient.Carbs}g | Fat: {ing.Ingredient.Fat}g | Kcal: {ing.Ingredient.Kcal} (Cantidad actual: {ing.Quantity}g)");
                    }
                }
            }
            promptBuilder.AppendLine(@"
                REGLAS ESTRICTAS Y SENCILLAS:
                1. Modifica SOLAMENTE la cantidad en gramos de los ingredientes principales (carne, arroz, pasta, etc.) para acercarte a los macros. No toques especias o cantidades de aceite muy peque
                2. MANT N RACIONES REALISTAS. Si te tienes que pasar o quedar corto por 50-100 Kcal o 10g de prote na, HAZLO. Es preferible que sobre/falte un poco antes que poner cantidades absurdas de comida.
                3. NO PIENSES DEMASIADO. Haz un ajuste rápido y matem tico. Tienes un margen de error MUY GRANDE (hasta 15% de diferencia).
                4. SIEMPRE devuelve isPossible: true.
                5. Devuelve EXCLUSIVAMENTE este JSON sin texto extra:
                {
                  ""isPossible"": true,
                  ""errorMessage"": """",
                  ""adjustedRecipes"": [
                    {
                      ""idFoodPlanDailyRecipe"": ID_FPDR,
                      ""ingredients"": [
                        { ""idIngredient"": ID_ING, ""newQuantityGrams"": NUEVA_CANTIDAD_ENTERA }
                      ]
                    }
                  ]
                }");
            return promptBuilder.ToString();
        }

        private async Task<DayAdjustmentAiResponseDto?> FetchAiAdjustments(string promptText)
        {
            string apiKey = _config["Gemini:ApiKey"] ?? string.Empty;
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";
            var requestBody = new { contents = new[] { new { parts = new[] { new { text = promptText } } } }, generationConfig = new { responseMimeType = "application/json", temperature = 0.2 } };
            var content = new StringContent(JsonSerializer.Serialize(requestBody, _jsonOptions), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Google Gemini ha rechazado la petición: {await response.Content.ReadAsStringAsync()}");
            }
            string responseString = await response.Content.ReadAsStringAsync();
            string aiResponseJson = JsonDocument.Parse(responseString)
                .RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? "";
            if (!string.IsNullOrWhiteSpace(aiResponseJson))
            {
                int startIndex = aiResponseJson.IndexOf('{');
                int endIndex = aiResponseJson.LastIndexOf('}');
                if (startIndex >= 0 && endIndex >= startIndex)
                {
                    aiResponseJson = aiResponseJson.Substring(startIndex, endIndex - startIndex + 1);
                }
                else
                {
                    throw new InvalidOperationException("Gemini no devolvi  un formato JSON v lido tras limpiarlo.");
                }
            }
            return JsonSerializer.Deserialize<DayAdjustmentAiResponseDto>(aiResponseJson, _jsonOptions);
        }

        private async Task ProcessAdjustedRecipe(AdjustedRecipeAiDto adjRecipe, UserFoodPlanDaily dailyPlan)
        {
            var originalFpdr = dailyPlan.FoodPlanDailyRecipes.FirstOrDefault(f => f.IdFoodPlanDailyRecipe == adjRecipe.idFoodPlanDailyRecipe);
            if (originalFpdr?.Recipe == null) return;
            var originalRecipe = originalFpdr.Recipe;
            string newRecipeName = (!string.IsNullOrEmpty(originalRecipe.RecipeName) && originalRecipe.RecipeName.StartsWith("[AJUSTADO]"))
                ? originalRecipe.RecipeName
                : "[AJUSTADO] " + originalRecipe.RecipeName;
            Recipe clonedRecipe = new Recipe
            {
                RecipeName = newRecipeName,
                IdMeal = originalRecipe.IdMeal,
                IdUser = originalRecipe.IdUser,
                RecipeDescription = originalRecipe.RecipeDescription
            };
            _context.Recipes.Add(clonedRecipe);
            await _context.SaveChangesAsync();
            foreach (AdjustedIngredientAiDto adjIngredient in adjRecipe.ingredients)
            {
                RecipeUserIngredient? origIngredient = originalRecipe.RecipeUserIngredients.FirstOrDefault(ri => ri.IdIngredient == adjIngredient.idIngredient);
                if (origIngredient != null)
                {
                    _context.RecipeUserIngredients.Add(new RecipeUserIngredient
                    {
                        IdRecipe = clonedRecipe.IdRecipe,
                        IdIngredient = adjIngredient.idIngredient,
                        Quantity = adjIngredient.newQuantityGrams,
                        Unit = "g"
                    });
                }
            }
            await _context.SaveChangesAsync();
            var clonedIngs = await _context.RecipeUserIngredients.Include(ri => ri.Ingredient).Where(ri => ri.IdRecipe == clonedRecipe.IdRecipe).ToListAsync();
            if (clonedIngs.Count > 0)
            {
                clonedRecipe.Protein = Math.Round(clonedIngs.Sum(ri => (ri.Quantity * (ri.Ingredient?.Protein ?? 0)) / 100.0), 2);
                clonedRecipe.Carbs = Math.Round(clonedIngs.Sum(ri => (ri.Quantity * (ri.Ingredient?.Carbs ?? 0)) / 100.0), 2);
                clonedRecipe.Fat = Math.Round(clonedIngs.Sum(ri => (ri.Quantity * (ri.Ingredient?.Fat ?? 0)) / 100.0), 2);
                clonedRecipe.TotalKcal = (int)Math.Round(clonedIngs.Sum(ri => (ri.Quantity * (ri.Ingredient?.Kcal ?? 0)) / 100.0), 0);
                await _context.SaveChangesAsync();
            }
            originalFpdr.IdRecipe = clonedRecipe.IdRecipe;
        }

        /// <summary>
        /// Motor de Inteligencia Artificial que analiza el bloque diario de recetas planificadas y aplica 
        /// un ajuste algorítmico y proporcional sobre los gramajes de sus ingredientes. Su finalidad es 
        /// garantizar la concordancia matemática exacta con los objetivos de macronutrientes preestablecidos del usuario.
        /// </summary>
        /// <param name="idUserFoodPlanDaily">Identificador de la jornada alimentaria sujeta a ajuste.</param>
        /// <returns>Interfaz de acción que indica el estado del re-cálculo y persistencia de las proporciones.</returns>
        [HttpPost("AdjustDayMacros/{idUserFoodPlanDaily}")]
        public async Task<IActionResult> AdjustDayMacros(int idUserFoodPlanDaily)
        {
            try
            {
                UserFoodPlanDaily? dailyPlan = await _context.UserFoodPlansDaily
                    .Include(d => d.FoodPlanDailyRecipes)
                        .ThenInclude(f => f.Recipe)
                            .ThenInclude(r => r.RecipeUserIngredients)
                                .ThenInclude(ri => ri.Ingredient)
                    .FirstOrDefaultAsync(d => d.IdUserFoodPlanDaily == idUserFoodPlanDaily);
                if (dailyPlan == null || dailyPlan.FoodPlanDailyRecipes.Count == 0)
                    return BadRequest("El d a no existe o no tiene recetas asignadas para ajustar.");
                string promptText = BuildAdjustmentPrompt(dailyPlan);
                DayAdjustmentAiResponseDto? aiResult = await FetchAiAdjustments(promptText);
                if (aiResult == null || !aiResult.isPossible)
                    return BadRequest(aiResult?.errorMessage ?? "Error procesando IA.");
                foreach (AdjustedRecipeAiDto? adjRecipe in aiResult.adjustedRecipes)
                {
                    if (adjRecipe != null)
                    {
                        await ProcessAdjustedRecipe(adjRecipe, dailyPlan);
                    }
                }
                await _context.SaveChangesAsync();
                var dayToUpdate = await _context.UserFoodPlansDaily
                    .Include(d => d.FoodPlanDailyRecipes)
                        .ThenInclude(f => f.Recipe)
                    .FirstOrDefaultAsync(d => d.IdUserFoodPlanDaily == idUserFoodPlanDaily);
                if (dayToUpdate != null)
                {
                    dayToUpdate.RealKcal = dayToUpdate.FoodPlanDailyRecipes.Sum(f => f.Recipe?.TotalKcal ?? 0);
                    dayToUpdate.RealProtein = Math.Round(dayToUpdate.FoodPlanDailyRecipes.Sum(f => f.Recipe?.Protein ?? 0), 1);
                    dayToUpdate.RealCarbs = Math.Round(dayToUpdate.FoodPlanDailyRecipes.Sum(f => f.Recipe?.Carbs ?? 0), 1);
                    dayToUpdate.RealFat = Math.Round(dayToUpdate.FoodPlanDailyRecipes.Sum(f => f.Recipe?.Fat ?? 0), 1);
                    await _context.SaveChangesAsync();
                }
                return Ok(new { message = "Macros ajustados correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"CRASH DE C# -> Mensaje: {ex.Message} || Rastreo: {ex.StackTrace}");
            }
        }
    }
}