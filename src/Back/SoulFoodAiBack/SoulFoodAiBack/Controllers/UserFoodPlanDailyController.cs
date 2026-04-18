using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace SoulFoodAiBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserFoodPlanDailyController : ControllerBase
    {
        private readonly DataContext _context;
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly IConfiguration _config;

        public UserFoodPlanDailyController(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet]
        [Route("GetDailyHeader/{idDailyPlan}")]
        public async Task<IActionResult> GetDailyHeader(int idDailyPlan)
        {
            
            UserFoodPlanDaily? day = await _context.UserFoodPlansDaily
                .Include(d => d.FoodPlan) 
                .Include(d => d.UserFoodPlanWeek)
                .FirstOrDefaultAsync(d => d.IdUserFoodPlanDaily == idDailyPlan);

            if (day == null)
                return NotFound("No se ha encontrado el día especificado.");

            
            CultureInfo culture = new CultureInfo("es-ES");
            string formattedDayName = day.CreationDate.ToString("dddd dd", culture);

            
            formattedDayName = char.ToUpper(formattedDayName[0]) + formattedDayName.Substring(1);


            DailyHeaderDto dto = new DailyHeaderDto
            {
                IdUserFoodPlanDaily = day.IdUserFoodPlanDaily,
                DietName = day.FoodPlan.FoodPlanName,
                DayName = formattedDayName,

                TargetKcal = day.TargetKcal,
                RealKcal = day.RealKcal,

                TargetProtein = Math.Round(day.TargetProtein, 1),
                RealProtein = Math.Round(day.RealProtein, 1),

                TargetCarbs = Math.Round(day.TargetCarbs, 1),
                RealCarbs = Math.Round(day.RealCarbs, 1),

                TargetFat = Math.Round(day.TargetFat, 1),
                RealFat = Math.Round(day.RealFat, 1),

                MealsPerDay = day.UserFoodPlanWeek.MealsPerDay
            };

            return Ok(dto);
        }

        [HttpPost]
        [Route("UpdateDailyRecipes")]
        public async Task<IActionResult> UpdateDailyRecipes([FromBody] UpdateDailyRecipesDto dto)
        {
           
            UserFoodPlanDaily? targetDay = await _context.UserFoodPlansDaily
                .FirstOrDefaultAsync(d => d.IdUserFoodPlanDaily == dto.IdUserFoodPlanDaily);

            if (targetDay == null)
                return NotFound("No se encontró el día especificado.");


            List<FoodPlanDailyRecipe> existingRecipes = await _context.FoodPlanDailyRecipes
                .Where(dr => dr.IdUserFoodPlanDaily == dto.IdUserFoodPlanDaily)
                .ToListAsync();

            if (existingRecipes.Any())
            {
                _context.FoodPlanDailyRecipes.RemoveRange(existingRecipes);
            }

            double totalRealKcal = 0;
            double totalRealProtein = 0;
            double totalRealCarbs = 0;
            double totalRealFat = 0;

            
            foreach (int recipeId in dto.RecipeIds)
            {
                
                Recipe? recipeInfo = await _context.Recipes.FindAsync(recipeId);

                if (recipeInfo != null)
                {
                    
                    await _context.FoodPlanDailyRecipes.AddAsync(new FoodPlanDailyRecipe
                    {
                        IdUserFoodPlanDaily = dto.IdUserFoodPlanDaily,
                        IdRecipe = recipeId
                    });

                   
                    totalRealKcal += recipeInfo.TotalKcal;
                    totalRealProtein += recipeInfo.Protein; 
                    totalRealCarbs += recipeInfo.Carbs;     
                    totalRealFat += recipeInfo.Fat;         
                }
            }

            targetDay.RealKcal = (int)totalRealKcal;
            targetDay.RealProtein = totalRealProtein;
            targetDay.RealCarbs = totalRealCarbs;
            targetDay.RealFat = totalRealFat;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Día actualizado correctamente con las nuevas recetas." });
        }

        [HttpPost("AdjustDayMacros/{idUserFoodPlanDaily}")]
        public async Task<IActionResult> AdjustDayMacros(int idUserFoodPlanDaily)
        {
            
            UserFoodPlanDaily? dailyPlan = await _context.UserFoodPlansDaily
                .Include(d => d.FoodPlanDailyRecipes)
                    .ThenInclude(f => f.Recipe)
                        .ThenInclude(r => r.RecipeUserIngredients)
                            .ThenInclude(ri => ri.Ingredient)
                .FirstOrDefaultAsync(d => d.IdUserFoodPlanDaily == idUserFoodPlanDaily);

            if (dailyPlan == null || !dailyPlan.FoodPlanDailyRecipes.Any())
                return BadRequest("El día no existe o no tiene recetas asignadas para ajustar.");

            var promptBuilder = new StringBuilder();
            promptBuilder.AppendLine("Eres un nutricionista experto. Tu objetivo es ajustar los gramos de los ingredientes de las recetas de un día para cuadrar los macros exactos.");
            promptBuilder.AppendLine($"\nOBJETIVOS DEL DÍA:");
            promptBuilder.AppendLine($"- Kcal: {dailyPlan.TargetKcal}");
            promptBuilder.AppendLine($"- Proteína: {dailyPlan.TargetProtein}g");
            promptBuilder.AppendLine($"- Carbohidratos: {dailyPlan.TargetCarbs}g");
            promptBuilder.AppendLine($"- Grasas: {dailyPlan.TargetFat}g\n");
            promptBuilder.AppendLine("RECETAS ACTUALES Y SUS INGREDIENTES (Macros por 100g):");

            foreach (FoodPlanDailyRecipe? fpdr in dailyPlan.FoodPlanDailyRecipes)
            {
                var r = fpdr.Recipe;
                promptBuilder.AppendLine($"Receta [ID_FPDR: {fpdr.IdFoodPlanDailyRecipe}]: {r.RecipeName}");
                foreach (var ing in r.RecipeUserIngredients)
                {
                    promptBuilder.AppendLine($"  - Ingrediente [ID_ING: {ing.IdIngredient}]: {ing.Ingredient.Name} | Prot: {ing.Ingredient.Protein}g | Carbs: {ing.Ingredient.Carbs}g | Fat: {ing.Ingredient.Fat}g | Kcal: {ing.Ingredient.Kcal} (Cantidad actual: {ing.Quantity}g)");
                }
            }

            promptBuilder.AppendLine(@"
            REGLAS ESTRICTAS:
            1. SOLO puedes modificar la cantidad en gramos de los ingredientes. No puedes añadir ni quitar ingredientes.
            2. RACIONES REALISTAS: Mantén la lógica culinaria (ej: no pongas 15g de pollo y 400g de arroz. Un chorrito de aceite son 5-15g, una ración de carne 100-250g, etc.).
            3. Si es IMPOSIBLE cuadrar los macros (con un margen de error del 10%) manteniendo raciones realistas, establece 'isPossible' en false y explica el porqué en 'errorMessage'.
            4. Devuelve ÚNICAMENTE este formato JSON:
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

            string apiKey = _config["Gemini:ApiKey"];
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            var requestBody = new { contents = new[] { new { parts = new[] { new { text = promptBuilder.ToString() } } } }, generationConfig = new { responseMimeType = "application/json", temperature = 0.2 } };
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode) return StatusCode(500, "Error con IA");

            string aiResponseJson = JsonDocument.Parse(await response.Content.ReadAsStringAsync())
                .RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

            DayAdjustmentAiResponseDto aiResult = JsonSerializer.Deserialize<DayAdjustmentAiResponseDto>(aiResponseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!aiResult.isPossible) return BadRequest(aiResult.errorMessage);

            foreach (AdjustedRecipeAiDto? adjRecipe in aiResult.adjustedRecipes)
            {
                var originalFpdr = dailyPlan.FoodPlanDailyRecipes.First(f => f.IdFoodPlanDailyRecipe == adjRecipe.idFoodPlanDailyRecipe);
                var originalRecipe = originalFpdr.Recipe;

                Recipe clonedRecipe = new Recipe
                {
                    RecipeName = originalRecipe.RecipeName.StartsWith("[AJUSTADO]") ? originalRecipe.RecipeName : "[AJUSTADO] " + originalRecipe.RecipeName,
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
                clonedRecipe.Protein = Math.Round(clonedIngs.Sum(ri => (ri.Quantity * ri.Ingredient.Protein) / 100.0), 2);
                clonedRecipe.Carbs = Math.Round(clonedIngs.Sum(ri => (ri.Quantity * ri.Ingredient.Carbs) / 100.0), 2);
                clonedRecipe.Fat = Math.Round(clonedIngs.Sum(ri => (ri.Quantity * ri.Ingredient.Fat) / 100.0), 2);
                clonedRecipe.TotalKcal = (int)Math.Round(clonedIngs.Sum(ri => (ri.Quantity * ri.Ingredient.Kcal) / 100.0), 0);
                await _context.SaveChangesAsync();

                originalFpdr.IdRecipe = clonedRecipe.IdRecipe;
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Macros ajustados correctamente." });
        }
    }
}
