using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using System.Globalization;
using System.Text.Json.Nodes;

namespace SoulFoodAiBack.Controllers
{
    /// <summary>
    /// Controlador responsable de la gestión integral de los ingredientes alimentarios.
    /// Orquesta las consultas al repositorio local, la integración con la API externa de OpenFoodFacts,
    /// y la administración de ingredientes genéricos y personalizados.
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
    public class IngredientController : ControllerBase
    {
        /// <summary>
        /// Contexto de acceso a datos inyectado para las operaciones CRUD en la base de datos relacional.
        /// </summary>
        private readonly DataContext _context;

        /// <summary>
        /// Constructor de la clase que inyecta la dependencia del contexto de base de datos.
        /// </summary>
        /// <param name="dataContext">Instancia del contexto de Entity Framework.</param>
        public IngredientController(DataContext dataContext)
        {
            _context = dataContext;
        }

        /// <summary>
        /// Recupera una colección de ingredientes filtrados por categoría, incluyendo tanto los genéricos del sistema como los personalizados por el usuario.
        /// </summary>
        /// <param name="category">Filtro de búsqueda por categoría principal del ingrediente.</param>
        /// <param name="userId">Identificador del usuario para obtener sus ingredientes específicos.</param>
        /// <returns>Colección de entidades ingrediente que coinciden con los criterios.</returns>
        [HttpGet]
        [Route("GetIngredients")]
        public async Task<ActionResult<List<Ingredient>>> GetIngredients([FromQuery] string category, [FromQuery] int userId)
        {
            string decodedCategory = System.Net.WebUtility.UrlDecode(category);
            List<Ingredient> ingredients = await _context.Ingredients.AsNoTracking()
        .Where(i => i.Category == decodedCategory
                 && !i.IsDeleted
                 && (i.CreatedByUserId == null || i.CreatedByUserId == userId || i.CreatedByUserId == 0))
                 .ToListAsync();
            return Ok(ingredients);
        }

        /// <summary>
        /// Consulta y devuelve la totalidad de los ingredientes disponibles y accesibles para un usuario específico.
        /// </summary>
        /// <param name="userId">Identificador primario del usuario.</param>
        /// <returns>Lista exhaustiva de ingredientes aplicables al usuario.</returns>
        [HttpGet]
        [Route("GetAllIngredients/{userId}")]
        public async Task<ActionResult<List<Ingredient>>> GetAllIngredients(int userId)
        {
            List<Ingredient> ingredients = await _context.Ingredients.AsNoTracking()
                            .Where(i => !i.IsDeleted
                            && (i.CreatedByUserId == null || i.CreatedByUserId == userId || i.CreatedByUserId == 0))
                            .ToListAsync();
            return Ok(ingredients);
        }

        private static bool ContainsAllSearchWords(string nameAndBrand, string searchText)
        {
            string[] searchWords = searchText.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (string word in searchWords)
            {
                if (!nameAndBrand.Contains(word))
                {
                    return false;
                }
            }
            return true;
        }

        private static void ExtractNutriments(JsonNode? nutriments, out double protein, out double carbs, out double fat, out double kcal)
        {
            protein = 0; carbs = 0; fat = 0; kcal = 0;
            if (nutriments != null)
            {
                if (nutriments["proteins_100g"] != null) double.TryParse(nutriments["proteins_100g"]!.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out protein);
                if (nutriments["carbohydrates_100g"] != null) double.TryParse(nutriments["carbohydrates_100g"]!.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out carbs);
                if (nutriments["fat_100g"] != null) double.TryParse(nutriments["fat_100g"]!.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out fat);
                if (nutriments["energy-kcal_100g"] != null) double.TryParse(nutriments["energy-kcal_100g"]!.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out kcal);
            }
        }

        private static void ProcessOpenFoodFactsProduct(JsonNode? product, string searchText, List<Ingredient> OFFResults)
        {
            if (product == null) return;
            string openFoodFactsId = product["_id"]?.ToString() ?? "";
            string name = product["product_name"]?.ToString() ?? "";
            string brand = product["brands"]?.ToString() ?? "";
            string imageUrl = product["image_front_small_url"]?.ToString() ?? product["image_url"]?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(name)) return;

            string nameAndBrand = (name + " " + brand).ToLower();
            if (!ContainsAllSearchWords(nameAndBrand, searchText)) return;

            ExtractNutriments(product["nutriments"], out double protein, out double carbs, out double fat, out double kcal);

            Ingredient newIngredient = new Ingredient
            {
                OpenFoodFactsId = openFoodFactsId,
                Name = name,
                Category = "",
                Brand = brand,
                ImageUrl = imageUrl,
                Protein = protein,
                Carbs = carbs,
                Fat = fat,
                Kcal = kcal
            };

            bool alreadyExists = OFFResults.Any(saved => string.Equals(saved.Name, newIngredient.Name, StringComparison.OrdinalIgnoreCase));
            if (!alreadyExists)
            {
                OFFResults.Add(newIngredient);
            }
        }

        private async Task<bool> TryFetchAndParseIngredients(HttpClient client, string url, string searchText, List<Ingredient> OFFResults)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JsonNode? data = JsonNode.Parse(jsonResponse);
                    if (data != null && data["products"] != null)
                    {
                        JsonArray productsArray = data["products"]!.AsArray();
                        if (productsArray.Count > 0)
                        {
                            foreach (JsonNode? product in productsArray)
                            {
                                ProcessOpenFoodFactsProduct(product, searchText, OFFResults);
                            }
                            return true;
                        }
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// Realiza una petición HTTP a la API externa de OpenFoodFacts para buscar productos alimentarios,
        /// procesando y adaptando la respuesta JSON al modelo de dominio interno de la aplicación.
        /// </summary>
        /// <param name="searchText">Cadena de texto utilizada como parámetro de búsqueda.</param>
        /// <returns>Colección de ingredientes con sus propiedades nutricionales estructuradas.</returns>
        [HttpGet]
        [Route("SearchOFFIngredients")]
        public async Task<ActionResult<List<Ingredient>>> SearchOFFIngredients([FromQuery] string searchText)
        {
            List<Ingredient> OFFResults = new List<Ingredient>();
            using HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(60);
            client.DefaultRequestHeaders.Add("User-Agent", "SoulFoodApp/1.0 (contact: info@soulfoodapp.com) DotNetBackend");
            string safeSearchText = Uri.EscapeDataString(searchText);
            string url = $"https://es.openfoodfacts.org/cgi/search.pl?search_terms={safeSearchText}&search_simple=1&action=process&json=1&page_size=60";
            int maxRetries = 6;
            int delayMilliseconds = 1500;

            for (int i = 0; i < maxRetries; i++)
            {
                bool success = await TryFetchAndParseIngredients(client, url, searchText, OFFResults);
                if (success && OFFResults.Count > 0)
                {
                    return Ok(OFFResults);
                }
                if (i < maxRetries - 1)
                {
                    await Task.Delay(delayMilliseconds);
                }
            }
            return Ok(OFFResults);
        }

        /// <summary>
        /// Registra un nuevo ingrediente personalizado creado de forma manual en el sistema.
        /// </summary>
        /// <param name="dto">Objeto de transferencia de datos con la información nutricional aportada por el usuario.</param>
        /// <returns>Código 200 (OK) en caso exitoso, o 400 (Bad Request) ante inconsistencia en los datos.</returns>
        [HttpPost]
        [Route("AddCustomIngredient")]
        public async Task<IActionResult> AddCustomIngredient([FromBody] CustomIngredientDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || dto.UserId <= 0)
            {
                return BadRequest("El nombre del alimento y el ID del usuario son obligatorios.");
            }
            Ingredient newIngredient = new Ingredient
            {
                Name = dto.Name,
                Brand = dto.Brand,
                Category = !string.IsNullOrWhiteSpace(dto.Category) ? dto.Category : "Personalizado",
                Icon = dto.Icon,
                ImageUrl = null,
                Protein = dto.Protein,
                Carbs = dto.Carbs,
                Fat = dto.Fat,
                Kcal = dto.Kcal,
                CreatedByUserId = dto.UserId
            };
            _context.Ingredients.Add(newIngredient);
            await _context.SaveChangesAsync();
            UserIngredient newFavorite = new UserIngredient
            {
                IdUser = dto.UserId,
                IdIngredient = newIngredient.IdIngredient
            };
            _context.UserIngredients.Add(newFavorite);
            await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Ejecuta un borrado lógico sobre un ingrediente personalizado, validando previamente los permisos de autoría del usuario solicitante.
        /// </summary>
        /// <param name="id">Identificador único del ingrediente.</param>
        /// <param name="userId">Identificador del usuario que intenta realizar la eliminación.</param>
        /// <returns>Mensaje de confirmación en caso de éxito, o código 403 (Forbidden) si no posee los permisos adecuados.</returns>
        [HttpDelete]
        [Route("DeleteCustomIngredient/{id}/{userId}")]
        public async Task<IActionResult> DeleteCustomIngredient(int id, int userId)
        {
            Ingredient? ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound("El ingrediente no existe.");
            }
            if (ingredient.CreatedByUserId == null || ingredient.CreatedByUserId != userId)
            {
                return Forbid("No tienes permiso para eliminar este ingrediente. Solo el creador puede hacerlo.");
            }
            ingredient.IsDeleted = true;
            await _context.SaveChangesAsync();
            return Ok(new { message = $"El ingrediente '{ingredient.Name}' ha sido eliminado de tu lista." });
        }

        /// <summary>
        /// Actualiza la información de un ingrediente personalizado garantizando que pertenece al usuario que realiza la petición.
        /// </summary>
        /// <param name="id">Identificador del ingrediente proporcionado en la ruta.</param>
        /// <param name="dto">Objeto de transferencia de datos con las modificaciones a aplicar.</param>
        /// <returns>Código 200 (OK) tras confirmar la actualización exitosa.</returns>
        [HttpPut]
        [Route("UpdateCustomIngredient/{dto}")]
        public async Task<IActionResult> UpdateCustomIngredient(int id, UpdateCustomIngredientDto dto)
        {
            if (dto.Id <= 0)
            {
                return BadRequest("El ID del ingrediente es obligatorio para actualizarlo.");
            }
            if (string.IsNullOrWhiteSpace(dto.Name) || dto.UserId <= 0)
            {
                return BadRequest("El nombre del alimento y el ID del usuario son obligatorios.");
            }
            Ingredient? ingredient = await _context.Ingredients.FindAsync(dto.Id);
            if (ingredient == null || ingredient.IsDeleted)
            {
                return NotFound("El ingrediente no existe o ha sido eliminado.");
            }
            if (ingredient.CreatedByUserId == null || ingredient.CreatedByUserId != dto.UserId)
            {
                return StatusCode(403, "No tienes permiso para editar este ingrediente. Solo el creador puede modificarlo.");
            }
            ingredient.Name = dto.Name;
            ingredient.Brand = dto.Brand;
            ingredient.Icon = dto.Icon;
            ingredient.Protein = dto.Protein;
            ingredient.Carbs = dto.Carbs;
            ingredient.Fat = dto.Fat;
            ingredient.Kcal = dto.Kcal;
            await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Persiste en el repositorio local un producto importado desde la API externa y establece su relación de disponibilidad con el usuario.
        /// </summary>
        /// <param name="dto">Objeto de transferencia con la información del ingrediente externo.</param>
        /// <returns>Interfaz de acción que indica el estado final de la operación.</returns>
        [HttpPost]
        [Route("AddSearchedIngredient")]
        public async Task<IActionResult> AddSearchedIngredient([FromBody] SaveSerchedIngredientDto dto)
        {
            if (dto.IdUser <= 0) return BadRequest("Usuario no v lido.");
            bool userExists = await _context.Users.AnyAsync(u => u.IdUser == dto.IdUser);
            if (!userExists) return NotFound("Usuario no existe.");
            Ingredient? ingredientSave = await _context.Ingredients
                .FirstOrDefaultAsync(i => i.OpenFoodFactsId == dto.IdOpenFoodFacts);
            if (ingredientSave == null)
            {
                ingredientSave = new Ingredient
                {
                    OpenFoodFactsId = dto.IdOpenFoodFacts,
                    Name = dto.Name ?? "Sin nombre",
                    Brand = dto.Brand,
                    ImageUrl = dto.ImageUrl,
                    Protein = dto.Protein,
                    Carbs = dto.Carbs,
                    Fat = dto.Fat,
                    Kcal = dto.Kcal,
                    Category = !string.IsNullOrWhiteSpace(dto.Category) ? dto.Category : "Otros",
                    CreatedByUserId = dto.IdUser,
                    IsDeleted = false
                };
                _context.Ingredients.Add(ingredientSave);
                await _context.SaveChangesAsync();
            }
            else if (ingredientSave.IsDeleted)
            {
                ingredientSave.IsDeleted = false;
                ingredientSave.Name = dto.Name ?? ingredientSave.Name;
                ingredientSave.Brand = dto.Brand ?? ingredientSave.Brand;
                ingredientSave.ImageUrl = dto.ImageUrl ?? ingredientSave.ImageUrl;
                ingredientSave.Category = !string.IsNullOrWhiteSpace(dto.Category) ? dto.Category : ingredientSave.Category;
                await _context.SaveChangesAsync();
            }
            bool relationExists = await _context.UserIngredients
                .AnyAsync(ui => ui.IdUser == dto.IdUser && ui.IdIngredient == ingredientSave.IdIngredient);
            if (!relationExists)
            {
                UserIngredient newUserIngredient = new UserIngredient
                {
                    IdUser = dto.IdUser,
                    IdIngredient = ingredientSave.IdIngredient
                };
                _context.UserIngredients.Add(newUserIngredient);
                await _context.SaveChangesAsync();
            }
            return Ok(new { message = "Ingrediente de cat logo añadido con éxito." });
        }

        /// <summary>
        /// Inserta un ingrediente de carácter global o genérico en el sistema, disponible para todos los usuarios.
        /// </summary>
        /// <param name="dto">Objeto de transferencia con las propiedades del nuevo ingrediente global.</param>
        /// <returns>Interfaz de acción que confirma el estado de la creación.</returns>
        [HttpPost]
        [Route("AddDefaultIngredient")]
        public async Task<IActionResult> AddIngredient(CreateIngredientDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("El nombre del alimento es obligatorio.");
            }
            Ingredient newIngredient = new Ingredient
            {
                Name = dto.Name,
                Brand = dto.Brand,
                OpenFoodFactsId = dto.OpenFoodFactsId,
                Category = dto.Category,
                SubCategory = dto.SubCategory,
                Icon = dto.Icon,
                ImageUrl = dto.ImageUrl,
                Protein = dto.Protein,
                Carbs = dto.Carbs,
                Fat = dto.Fat,
                Kcal = dto.Kcal,
                CreatedByUserId = dto.CreatedByUserId,
                IsDeleted = dto.IsDeleted
            };
            _context.Ingredients.Add(newIngredient);
            await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Procesa una lista de ingredientes genéricos para insertarlos de forma masiva en la base de datos.
        /// </summary>
        /// <param name="dtos">Colección de objetos de transferencia con la información a insertar.</param>
        /// <returns>Estado de la operación junto a la cantidad de registros introducidos.</returns>
        [HttpPost]
        [Route("AddDefaultIngredients")]
        public async Task<IActionResult> AddIngredients([FromBody] List<CreateIngredientDto> dtos)
        {
            if (dtos == null || dtos.Count == 0)
            {
                return BadRequest("La lista de ingredientes no puede estar vac a.");
            }
            if (dtos.Any(d => string.IsNullOrWhiteSpace(d.Name)))
            {
                return BadRequest("Todos los alimentos deben tener un nombre obligatorio.");
            }
            List<Ingredient>? newIngredients = dtos.Select(dto => new Ingredient
            {
                Name = dto.Name,
                Brand = dto.Brand,
                OpenFoodFactsId = dto.OpenFoodFactsId,
                Category = dto.Category,
                SubCategory = dto.SubCategory,
                Icon = dto.Icon,
                ImageUrl = dto.ImageUrl,
                Protein = dto.Protein,
                Carbs = dto.Carbs,
                Fat = dto.Fat,
                Kcal = dto.Kcal,
                CreatedByUserId = dto.CreatedByUserId,
                IsDeleted = dto.IsDeleted
            }).ToList();
            _context.Ingredients.AddRange(newIngredients);
            await _context.SaveChangesAsync();
            return Ok(new { message = $"Se han añadido {newIngredients.Count} ingredientes correctamente." });
        }

        /// <summary>
        /// Actualiza de forma específica la URL de la imagen fotográfica asociada a un ingrediente en la base de datos.
        /// </summary>
        /// <param name="dto">Objeto de transferencia que contiene el identificador y la nueva URL de la imagen.</param>
        /// <returns>Interfaz de acción que indica el resultado del proceso de actualización.</returns>
        [HttpPut]
        [Route("UpdateImagenDefaultIngredient")]
        public async Task<IActionResult> UpdateIngredient(UpdateImageIngredientDto dto)
        {
            Ingredient? ingredient = await _context.Ingredients.FindAsync(dto.IdIngredient);
            if (ingredient != null)
            {
                ingredient.ImageUrl = dto.ImageUrl;
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        /// <summary>
        /// Ejecuta una actualización masiva de las URLs de las imágenes fotográficas para un conjunto determinado de ingredientes.
        /// </summary>
        /// <param name="dtos">Colección de objetos de transferencia con los pares de identificador y URL.</param>
        /// <returns>Estado de la operación y recuento del total de registros afectados.</returns>
        [HttpPut]
        [Route("UpdateImageIngredients")]
        public async Task<IActionResult> UpdateIngredients([FromBody] List<UpdateImageIngredientDto> dtos)
        {
            if (dtos == null || dtos.Count == 0)
            {
                return BadRequest("La lista de ingredientes está vac a.");
            }
            var idsToUpdate = dtos.Select(d => d.IdIngredient).ToList();
            var ingredients = await _context.Ingredients
                .Where(i => idsToUpdate.Contains(i.IdIngredient))
                .ToListAsync();
            foreach (var ingredient in ingredients)
            {
                var newImageData = dtos.FirstOrDefault(d => d.IdIngredient == ingredient.IdIngredient);
                if (newImageData != null && ingredient != null)
                {
                    ingredient.ImageUrl = newImageData.ImageUrl;
                }
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = $"{ingredients.Count} im genes actualizadas correctamente." });
        }

        /// <summary>
        /// Recupera el listado de ingredientes que un usuario ha marcado explícitamente como permitidos o favoritos en su perfil nutricional.
        /// </summary>
        /// <param name="idUser">Identificador primario del usuario a consultar.</param>
        /// <returns>Colección de objetos anónimos con los identificadores y nombres de los ingredientes permitidos.</returns>
        [HttpGet]
        [Route("GetAllowedIngredients/{idUser}")]
        public async Task<IActionResult> GetAllowedIngredients(int idUser)
        {
            var userIngredients = await _context.UserIngredients
                .Include(ui => ui.Ingredient)
                .Where(ui => ui.IdUser == idUser && ui.Ingredient != null && !ui.Ingredient.IsDeleted)
                .Select(ui => new
                {
                    idIngredient = ui.Ingredient!.IdIngredient,
                    name = ui.Ingredient.Name
                })
                .ToListAsync();
            return Ok(userIngredients);
        }
    }
}