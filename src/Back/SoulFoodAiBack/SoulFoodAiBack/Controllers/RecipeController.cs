using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using System.Text;
using System.Text.Json;

namespace SoulFoodAiBack.Controllers
{
    /// <summary>
    /// Controlador principal para la gestión de recetas. Orquesta las operaciones sobre recetas de creación manual 
    /// y actúa como motor de integración con la Inteligencia Artificial (Gemini) para la generación dinámica de recetas.
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
    public class RecipeController : ControllerBase
    {
        /// <summary>
        /// Contexto de base de datos para la persistencia y recuperación de datos relacionales.
        /// </summary>
        private readonly DataContext _context;

        /// <summary>
        /// Cliente HTTP estático utilizado para gestionar las peticiones salientes hacia la API de Inteligencia Artificial.
        /// </summary>
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Proveedor de configuración inyectado para acceder a variables de entorno, como claves de API.
        /// </summary>
        private readonly IConfiguration _config;
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        /// <summary>
        /// Constructor de la clase que inyecta las dependencias necesarias de contexto de base de datos y configuración.
        /// </summary>
        /// <param name="context">Instancia del contexto de base de datos.</param>
        /// <param name="config">Proveedor de configuración de la aplicación.</param>
        public RecipeController(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        /// <summary>
        /// Consulta y recupera las recetas activas vinculadas a un usuario, estructuradas en un formato ligero (tarjeta) con información de macronutrientes.
        /// </summary>
        /// <param name="idUser">Identificador único del usuario propietario.</param>
        /// <returns>Colección de objetos de transferencia de tipo RecipeCardDto.</returns>
        [HttpGet]
        [Route("GetRecipesForUser/{idUser}")]
        public async Task<ActionResult<List<RecipeCardDto>>> GetRecipesForUser(int idUser)
        {
            List<RecipeCardDto>? userRecipes = await _context.Recipes
                .Include(r => r.Meal)
                .Include(r => r.User)
                    .ThenInclude(u => u.UserData)
                        .ThenInclude(ud => ud.FoodPlan)
                .Include(r => r.RecipeUserIngredients)
                    .ThenInclude(ri => ri.Ingredient)
                .Where(r => r.IdUser == idUser && r.IsActive && (!string.IsNullOrEmpty(r.RecipeName) && !r.RecipeName.StartsWith("[AJUSTADO]")))
                .Select(r => new RecipeCardDto
                {
                    IdRecipe = r.IdRecipe,
                    RecipeName = r.RecipeName,
                    Kcal = r.TotalKcal,
                    MealName = r.Meal != null ? r.Meal.MealName : "",
                    DietName = r.User != null && r.User.UserData != null && r.User.UserData.FoodPlan != null ? r.User.UserData.FoodPlan.FoodPlanName : "",
                    Protein = r.Protein,
                    Carbs = r.Carbs,
                    Fat = r.Fat,
                    RecipeDescription = r.RecipeDescription,
                    Ingredients = r.RecipeUserIngredients.Select(ri => new RecipeIngredientDetailDto
                    {
                        Name = ri.Ingredient != null ? ri.Ingredient.Name : "",
                        Quantity = ri.Quantity,
                        Unit = ri.Unit
                    }).ToList()
                })
                .ToListAsync();
            return Ok(userRecipes);
        }

        /// <summary>
        /// Registra en la base de datos una nueva receta creada manualmente por un usuario, incluyendo sus ingredientes y cantidades exactas.
        /// </summary>
        /// <param name="idUser">Identificador del usuario autor de la receta.</param>
        /// <param name="dto">Objeto de transferencia que contiene la formulación completa de la receta.</param>
        /// <returns>Código 200 (OK) tras confirmar la correcta inserción en la base de datos.</returns>
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
            recipeAdd.Protein = Math.Round(ingredientesDeLaReceta.Sum(ri => (ri.Quantity * (ri.Ingredient?.Protein ?? 0)) / 100.0), 2);
            recipeAdd.Carbs = Math.Round(ingredientesDeLaReceta.Sum(ri => (ri.Quantity * (ri.Ingredient?.Carbs ?? 0)) / 100.0), 2);
            recipeAdd.Fat = Math.Round(ingredientesDeLaReceta.Sum(ri => (ri.Quantity * (ri.Ingredient?.Fat ?? 0)) / 100.0), 2);
            recipeAdd.TotalKcal = (int)Math.Round(ingredientesDeLaReceta.Sum(ri => (ri.Quantity * (ri.Ingredient?.Kcal ?? 0)) / 100.0), 0);
            await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Ejecuta el motor de Inteligencia Artificial para generar una receta personalizada. 
        /// Combina los parámetros del usuario (dieta, objetivo, intolerancias) y formula un prompt estructurado 
        /// para que la IA diseñe un plato equilibrado y retorne los datos estructurados en formato JSON.
        /// </summary>
        /// <param name="idUser">Identificador del usuario objetivo para la receta.</param>
        /// <param name="dto">Objeto de transferencia con los requisitos específicos del plato a generar.</param>
        /// <returns>Objeto con el identificador y nombre de la receta generada, o un mensaje de error detallado en caso de fallo.</returns>
        [HttpPost]
        [Route("CreateRecipeAI/{idUser}")]
        public async Task<IActionResult> CreateRecipeAI(int idUser, CreateAiRecipeDto dto)
        {
            User? user = await _context.Users.FindAsync(idUser);
            Meal? meal = await _context.Meals.FindAsync(dto.IdMeal);
            UserData? userData = await _context.UserDatas
                                                .Include(g => g.Goal).Include(f => f.FoodPlan)
                                                .FirstOrDefaultAsync(ud => ud.IdUser == idUser);
            if (user == null || meal == null || userData == null || userData.FoodPlan == null || userData.Goal == null)
            {
                return BadRequest("Usuario, comida o dieta no v lidos.");
            }
            var allowedIngredients = await _context.UserIngredients
                                             .Include(ui => ui.Ingredient)
                                             .Where(ui => ui.IdUser == idUser && ui.Ingredient != null && !ui.Ingredient.IsDeleted)
                                             .Select(ui => new { ui.Ingredient!.IdIngredient, ui.Ingredient.Name })
                                             .ToListAsync();
            if (allowedIngredients.Count == 0) { return BadRequest("Debes proporcionar ingredientes v lidos."); }
            string listIngredientesAviables = string.Join(", ", allowedIngredients.Select(i => $"[ID: {i.IdIngredient}] {i.Name}"));
            var userIntolerances = await _context.UserIntolerances
                                     .Include(ui => ui.Intolerance)
                                     .Where(ui => ui.IdUser == idUser)
                                     .Select(ui => ui.Intolerance!.IntoleranceName)
                                     .ToListAsync();
            string intoleranciasText = userIntolerances.Count > 0
                    ? $"ATENCIÓN CRÍTICA: El usuario tiene las siguientes intolerancias o alergias: {string.Join(", ", userIntolerances)}. BAJO NING N CONCEPTO puedes usar nada que entre en conflicto con esto."
                    : "El usuario no tiene intolerancias registradas.";
            string peticionUsuario = string.IsNullOrWhiteSpace(dto.PromptDescription)
                    ? "No hay petición específica. Crea una receta equilibrada, con sentido culinario y deliciosa por tu cuenta."
                    : $"ATENCI N - PETICI N ESPEC FICA DEL USUARIO (Dale máxima prioridad): {dto.PromptDescription}";
            string promptAI = $@"
            Eres un chef experto en nutrición y dietética, especialista en diseñar recetas combinando ingredientes y ajustando los macronutrientes según diferentes objetivos.
            
            Tu tarea es crear una receta para la comida: {meal.MealName}.
            Debe ser estrictamente compatible con la dieta: {userData.FoodPlan.FoodPlanName}.
            Y ten encuenta que el objetivo de esta persona es: {userData.Goal.GoalName}.
            
            {intoleranciasText}
            
            INSTRUCCIONES DE DISE O DE RECETA:
            - S  consecuente y l gico. Las recetas pueden ser elaboraciones complejas o platos muy sencillos del d a a d a, pero siempre deben ser equilibradas y cumplir con la dieta y alergias asignadas.
            - {peticionUsuario}
            
            REGLA ESTRICTA DE INGREDIENTES: 
            SOLO PUEDES USAR INGREDIENTES DE ESTA LISTA EXACTA: {listIngredientesAviables}.
            Bajo ninguna circunstancia incluyas un ingrediente que no está en la lista (solo se puede añadir agua y sal. El aceite si es imprescindible y las especias sióno aparecen ah  solo se pueden añadir si son opcionales marcansolas con (opcional)).
            
            MANEJO DE CASOS IMPOSIBLES ( MUY IMPORTANTE!):
            Si es f sicamente IMPOSIBLE crear una receta que cumpla la dieta Y las intolerancias usando SOLO esos ingredientes (ej: si es cel aco y solo tiene pan de trigo), NO inventes la receta. Devuelve EXACTAMENTE este JSON:
            {{
                ""RecipeName"": ""ERROR"",
                ""RecipeDescription"": ""Explica aqu  brevemente al usuario por qu  no puedes crear la receta (ej: 'No puedo cocinar esto porque los ingredientes chocan con tu intolerancia al gluten')."",
                ""Ingredients"": []
            }}
            
            Si S  es posible, devuelve la receta  NICAMENTE siguiendo esta estructura JSON exacta. No a adas texto antes ni después, y no uses bloques de c digo Markdown:
            {{
                ""RecipeName"": ""Nombre creativo y apetecible de la receta"",
                ""RecipeDescription"": ""Instrucciones cortas y claras de c mo preparar la receta paso a paso"",
                ""Ingredients"": [
                    {{
                        ""IdIngredient"": ID_NUMERICO,
                        ""QuantityInGrams"": CANTIDAD_NUMERICA_ENTERA
                    }}
                ]
            }}";
            string apiKey = _config["Gemini:ApiKey"] ?? string.Empty;
            if (string.IsNullOrEmpty(apiKey))
                return StatusCode(500, "Falta la API Key de Gemini en appsettings.json");
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";
            var requestBody = new
            {
                contents = new[] { new { parts = new[] { new { text = promptAI } } } },
                generationConfig = new { responseMimeType = "application/json", temperature = 0.7 }
            };
            string jsonRequestBody = JsonSerializer.Serialize(requestBody, _jsonOptions);
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
                    .GetString() ?? "";
                if (!string.IsNullOrEmpty(aiResponseJson))
                {
                    aiResponseJson = aiResponseJson.Replace("```json", "").Replace("```", "").Trim();
                }
            }
            catch (Exception)
            {
                return StatusCode(500, "Error al leer la respuesta de la IA.");
            }
            AIRecipeResponseDto? aiRecipe;
            try
            {
                aiRecipe = JsonSerializer.Deserialize<AIRecipeResponseDto>(aiResponseJson, _jsonOptions);
            }
            catch
            {
                return StatusCode(500, "La IA devolvi  un formato inv lido. Revisa la consola o intenta de nuevo.");
            }
            if (aiRecipe == null)
            {
                return StatusCode(500, "Fallo al procesar la receta de la IA.");
            }
            if (string.Equals(aiRecipe.RecipeName, "ERROR", StringComparison.OrdinalIgnoreCase))
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
            var validIngredients = aiRecipe.Ingredients
                .Where(ing => allowedIngredients.Any(a => a.IdIngredient == ing.IdIngredient))
                .Select(ing => new RecipeUserIngredient
                {
                    IdRecipe = recipeAdd.IdRecipe,
                    IdIngredient = ing.IdIngredient,
                    Quantity = ing.QuantityInGrams,
                    Unit = "g"
                });
            _context.RecipeUserIngredients.AddRange(validIngredients);
            await _context.SaveChangesAsync();
            var ingredientesReceta = await _context.RecipeUserIngredients
                .Include(ri => ri.Ingredient)
                .Where(ri => ri.IdRecipe == recipeAdd.IdRecipe)
                .ToListAsync();
            recipeAdd.Protein = Math.Round(ingredientesReceta.Sum(ri => (ri.Quantity * (ri.Ingredient?.Protein ?? 0)) / 100.0), 2);
            recipeAdd.Carbs = Math.Round(ingredientesReceta.Sum(ri => (ri.Quantity * (ri.Ingredient?.Carbs ?? 0)) / 100.0), 2);
            recipeAdd.Fat = Math.Round(ingredientesReceta.Sum(ri => (ri.Quantity * (ri.Ingredient?.Fat ?? 0)) / 100.0), 2);
            recipeAdd.TotalKcal = (int)Math.Round(ingredientesReceta.Sum(ri => (ri.Quantity * (ri.Ingredient?.Kcal ?? 0)) / 100.0), 0);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                Message = "Receta generada con exito",
                RecipeId = recipeAdd.IdRecipe,
                RecipeName = recipeAdd.RecipeName
            });
        }

        /// <summary>
        /// Ejecuta un borrado lógico (archivado) sobre una receta para retirarla de la vista activa del usuario sin perder su histórico.
        /// </summary>
        /// <param name="idRecipe">Identificador primario de la receta a ocultar.</param>
        /// <returns>Mensaje de confirmación de la eliminación lógica.</returns>
        [HttpPut("ArchiveRecipe/{idRecipe}")]
        public async Task<IActionResult> ArchiveRecipe(int idRecipe)
        {
            Recipe? recipe = await _context.Recipes.FindAsync(idRecipe);
            if (recipe == null) { return NotFound(new { message = "Receta no encontrada." }); }
            recipe.IsActive = false;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Receta eliminada correctamente." });
        }
    }
}