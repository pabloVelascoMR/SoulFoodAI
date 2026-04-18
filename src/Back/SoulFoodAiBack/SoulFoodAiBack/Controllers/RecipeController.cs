using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;

namespace SoulFoodAiBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeController : ControllerBase
    {
        private readonly DataContext _context;
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly IConfiguration _config;

        public RecipeController(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpGet]
        [Route("GetRecipesForUser/{idUser}")]
        public async Task<ActionResult<List<RecipeCardDto>>> GetRecipesForUser(int idUser)
        {
            var userRecipes = await _context.Recipes
                .Include(r => r.Meal)
                .Include(r => r.User)
                    .ThenInclude(u => u.UserData)
                        .ThenInclude(ud => ud.FoodPlan)
               
                .Include(r => r.RecipeUserIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Where(r => r.IdUser == idUser)
                .Select(r => new RecipeCardDto
                {
                    IdRecipe = r.IdRecipe,
                    RecipeName = r.RecipeName,
                    Kcal = r.TotalKcal,
                    MealName = r.Meal.MealName,
                    DietName = r.User.UserData.FoodPlan.FoodPlanName,
                    Protein = r.Protein,
                    Carbs = r.Carbs,
                    Fat = r.Fat,
                    RecipeDescription = r.RecipeDescription,

                    
                    Ingredients = r.RecipeUserIngredients.Select(ri => new RecipeIngredientDetailDto
                    {
                        Name = ri.Ingredient.Name, 
                        Quantity = ri.Quantity,
                        Unit = ri.Unit
                    }).ToList()
                })
                .ToListAsync();

            return Ok(userRecipes);
        }

        [HttpPost]
        [Route("AddRecipesForUser/{idUser}")]
        public async Task<IActionResult> AddRecipesForUser(int idUser, AddRecipeDto dto)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.IdUser == idUser);
            if (user is null) { return NotFound("Ese usuario no existe."); }

            Recipe? recipe = _context.Recipes
                                .Where(r => r.RecipeName == dto.RecipeName)
                                .Where(r => r.IdUser == idUser)
                                .FirstOrDefault();

            if (recipe != null) { return BadRequest("Esta receta con este nombre ya existe."); }

            Recipe recipeAdd = new Recipe
            {
                IdMeal = dto.IdMeal,
                RecipeName = dto.RecipeName,
                IdUser = idUser,
                RecipeDescription = dto.RecipeDescription,
            };

            _context.Recipes.Add(recipeAdd);
            await _context.SaveChangesAsync();

            var allowedIngredientIds = await _context.UserIngredients
                                            .Where(ui => ui.IdUser == idUser)
                                            .Select(ui => ui.IdIngredient)
                                            .ToListAsync();

            int i = 0;
            foreach (int IdIngredient in dto.IdIngredients)
            {
                if (allowedIngredientIds.Contains(IdIngredient))
                {
                    RecipeUserIngredient recipeIngredient = new RecipeUserIngredient
                    {
                        IdRecipe = recipeAdd.IdRecipe,
                        IdIngredient = IdIngredient,
                        Quantity = dto.Quantity.ElementAt(i),
                        Unit = dto.Unit.ElementAt(i)
                    };
                    _context.RecipeUserIngredients.Add(recipeIngredient);
                }
                i++;
            }
            await _context.SaveChangesAsync();

            List<RecipeUserIngredient>? ingredientesDeLaReceta = await _context.RecipeUserIngredients
                                                .Include(ri => ri.Ingredient)
                                                .Where(ri => ri.IdRecipe == recipeAdd.IdRecipe)
                                                .ToListAsync();

            recipeAdd.Protein = Math.Round(ingredientesDeLaReceta.Sum(ri => (ri.Quantity * ri.Ingredient.Protein) / 100.0), 2);
            recipeAdd.Carbs = Math.Round(ingredientesDeLaReceta.Sum(ri => (ri.Quantity * ri.Ingredient.Carbs) / 100.0), 2);
            recipeAdd.Fat = Math.Round(ingredientesDeLaReceta.Sum(ri => (ri.Quantity * ri.Ingredient.Fat) / 100.0), 2);
            recipeAdd.TotalKcal = (int)Math.Round(ingredientesDeLaReceta.Sum(ri => (ri.Quantity * ri.Ingredient.Kcal) / 100.0), 0);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Route("CreateRecipeAI/{idUser}")]
        public async Task<IActionResult> CreateRecipeAI(int idUser, CreateAiRecipeDto dto)
        {
            User? user = await _context.Users.FindAsync(idUser);
            Meal? meal = await _context.Meals.FindAsync(dto.IdMeal);

            UserData? userData = await _context.UserDatas
                                                .Include(g => g.Goal).Include(f => f.FoodPlan)
                                                .FirstOrDefaultAsync(ud => ud.IdUser == idUser);

            if (user == null || meal == null || userData == null) { return BadRequest("Usuario, comida o dieta no válidos."); }

            var allowedIngredients = await _context.UserIngredients
                                             .Include(ui => ui.Ingredient)
                                             .Where(ui => ui.IdUser == idUser && !ui.Ingredient.IsDeleted)
                                             .Select(ui => new { ui.Ingredient.IdIngredient, ui.Ingredient.Name })
                                             .ToListAsync();

            if (!allowedIngredients.Any()) { return BadRequest("Debes proporcionar ingredientes válidos."); }

            string listIngredientesAviables = string.Join(", ", allowedIngredients.Select(i => $"[ID: {i.IdIngredient}] {i.Name}"));

            var userIntolerances = await _context.UserIntolerances
                                     .Include(ui => ui.Intolerance)
                                     .Where(ui => ui.IdUser == idUser)
                                     .Select(ui => ui.Intolerance.IntoleranceName)
                                     .ToListAsync();

            string intoleranciasText = userIntolerances.Any()
                    ? $"ATENCIÓN CRÍTICA: El usuario tiene las siguientes intolerancias o alergias: {string.Join(", ", userIntolerances)}. BAJO NINGÚN CONCEPTO puedes usar nada que entre en conflicto con esto."
                    : "El usuario no tiene intolerancias registradas.";

            string peticionUsuario = string.IsNullOrWhiteSpace(dto.PromptDescription)
                    ? "No hay petición específica. Crea una receta equilibrada, con sentido culinario y deliciosa por tu cuenta."
                    : $"ATENCIÓN - PETICIÓN ESPECÍFICA DEL USUARIO (Dale máxima prioridad): {dto.PromptDescription}";

            string promptAI = $@"
            Eres un chef experto en nutrición y dietética, especialista en diseñar recetas combinando ingredientes y ajustando los macronutrientes según diferentes objetivos.
            
            Tu tarea es crear una receta para la comida: {meal.MealName}.
            Debe ser estrictamente compatible con la dieta: {userData.FoodPlan.FoodPlanName}.
            Y ten encuenta que el objetivo de esta persona es: {userData.Goal.GoalName}.
            
            {intoleranciasText}
            
            INSTRUCCIONES DE DISEÑO DE RECETA:
            - Sé consecuente y lógico. Las recetas pueden ser elaboraciones complejas o platos muy sencillos del día a día, pero siempre deben ser equilibradas y cumplir con la dieta y alergias asignadas.
            - {peticionUsuario}
            
            REGLA ESTRICTA DE INGREDIENTES: 
            SOLO PUEDES USAR INGREDIENTES DE ESTA LISTA EXACTA: {listIngredientesAviables}.
            Bajo ninguna circunstancia incluyas un ingrediente que no esté en la lista (solo se puede añadir agua y sal. El aceite si es imprescindible y las especias si no aparecen ahí solo se pueden añadir si son opcionales marcansolas con (opcional)).
            
            MANEJO DE CASOS IMPOSIBLES (¡MUY IMPORTANTE!):
            Si es físicamente IMPOSIBLE crear una receta que cumpla la dieta Y las intolerancias usando SOLO esos ingredientes (ej: si es celíaco y solo tiene pan de trigo), NO inventes la receta. Devuelve EXACTAMENTE este JSON:
            {{
                ""RecipeName"": ""ERROR"",
                ""RecipeDescription"": ""Explica aquí brevemente al usuario por qué no puedes crear la receta (ej: 'No puedo cocinar esto porque los ingredientes chocan con tu intolerancia al gluten')."",
                ""Ingredients"": []
            }}
            
            Si SÍ es posible, devuelve la receta ÚNICAMENTE siguiendo esta estructura JSON exacta. No añadas texto antes ni después, y no uses bloques de código Markdown:
            {{
                ""RecipeName"": ""Nombre creativo y apetecible de la receta"",
                ""RecipeDescription"": ""Instrucciones cortas y claras de cómo preparar la receta paso a paso"",
                ""Ingredients"": [
                    {{
                        ""IdIngredient"": ID_NUMERICO,
                        ""QuantityInGrams"": CANTIDAD_NUMERICA_ENTERA
                    }}
                ]
            }}";

            string apiKey = _config["Gemini:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                return StatusCode(500, "Falta la API Key de Gemini en appsettings.json");

            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

            var requestBody = new
            {
                contents = new[] { new { parts = new[] { new { text = promptAI } } } },
                generationConfig = new { responseMimeType = "application/json", temperature = 0.7 }
            };

            string jsonRequestBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();
                return StatusCode(500, $"Error conectando con Gemini: {error}");
            }

            string responseString = await response.Content.ReadAsStringAsync();

            string aiResponseJson;
            try
            {
                using JsonDocument doc = JsonDocument.Parse(responseString);
                aiResponseJson = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                if (aiResponseJson != null)
                {
                    aiResponseJson = aiResponseJson.Replace("```json", "").Replace("```", "").Trim();
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Error al leer la respuesta de la IA.");
            }

            AIRecipeResponseDto aiRecipe;
            try
            {
                JsonSerializerOptions? options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                aiRecipe = JsonSerializer.Deserialize<AIRecipeResponseDto>(aiResponseJson, options);
            }
            catch
            {
                return StatusCode(500, "La IA devolvió un formato inválido. Revisa la consola o intenta de nuevo.");
            }

            if (aiRecipe.RecipeName.ToUpper() == "ERROR")
            {
                return BadRequest(new { message = aiRecipe.RecipeDescription });
            }

            Recipe recipeAdd = new Recipe
            {
                IdMeal = dto.IdMeal,
                RecipeName = aiRecipe.RecipeName,
                IdUser = idUser,
                RecipeDescription = aiRecipe.RecipeDescription,
            };

            _context.Recipes.Add(recipeAdd);
            await _context.SaveChangesAsync();

            foreach (var ing in aiRecipe.Ingredients)
            {
                if (allowedIngredients.Any(a => a.IdIngredient == ing.IdIngredient))
                {
                    _context.RecipeUserIngredients.Add(new RecipeUserIngredient
                    {
                        IdRecipe = recipeAdd.IdRecipe,
                        IdIngredient = ing.IdIngredient,
                        Quantity = ing.QuantityInGrams,
                        Unit = "g"
                    });
                }
            }
            await _context.SaveChangesAsync();

            var ingredientesReceta = await _context.RecipeUserIngredients
                .Include(ri => ri.Ingredient)
                .Where(ri => ri.IdRecipe == recipeAdd.IdRecipe)
                .ToListAsync();

            recipeAdd.Protein = Math.Round(ingredientesReceta.Sum(ri => (ri.Quantity * ri.Ingredient.Protein) / 100.0), 2);
            recipeAdd.Carbs = Math.Round(ingredientesReceta.Sum(ri => (ri.Quantity * ri.Ingredient.Carbs) / 100.0), 2);
            recipeAdd.Fat = Math.Round(ingredientesReceta.Sum(ri => (ri.Quantity * ri.Ingredient.Fat) / 100.0), 2);
            recipeAdd.TotalKcal = (int)Math.Round(ingredientesReceta.Sum(ri => (ri.Quantity * ri.Ingredient.Kcal) / 100.0), 0);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Receta generada con éxito",
                RecipeId = recipeAdd.IdRecipe,
                RecipeName = recipeAdd.RecipeName
            });
        }
    }
}
    
