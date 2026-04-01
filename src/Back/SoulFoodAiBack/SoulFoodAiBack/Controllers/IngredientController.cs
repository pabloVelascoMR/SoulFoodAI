using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Services;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using System.Globalization;
using System.Text.Json.Nodes;

namespace SoulFoodAiBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientController : ControllerBase
    {
        private readonly DataContext _context;

        public IngredientController(DataContext dataContext)
        {
            _context = dataContext;
        }

        [HttpGet]
        [Route("GetIngredients/{category}/{userId}")]
        public async Task<ActionResult<List<Ingredient>>> GetIngredients(string category,int userId)
        {
            List<Ingredient> ingredients = await _context.Ingredients
                       .Where(i => i.Category.ToLower() == category.ToLower()&& !i.IsDeleted && (i.CreatedByUserId == null || i.CreatedByUserId == userId))
                       .ToListAsync();

            return Ok(ingredients);
        }

        [HttpGet]
        [Route("GetAllIngredients/{userId}")]
        public async Task<ActionResult<List<Ingredient>>> GetAllIngredients(int userId)
        {
            List<Ingredient> ingredients = await _context.Ingredients
                            .Where( i => !i.IsDeleted
                            && (i.CreatedByUserId == null || i.CreatedByUserId == userId))
                            .ToListAsync();

            return Ok(ingredients);
        }

        [HttpGet]
        [Route("SearchOFFIngredients/{searchText}")]
        public async Task<ActionResult<List<Ingredient>>> SearchOFFIngredients(string searchText)
        {
            
            List<Ingredient> OFFResults = new List<Ingredient>();
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(60);

            client.DefaultRequestHeaders.Add("User-Agent", "SoulFoodApp/1.0 (contact: info@soulfoodapp.com) DotNetBackend");

            string safeSearchText = Uri.EscapeDataString(searchText);

            string url = $"https://es.openfoodfacts.org/cgi/search.pl?search_terms={safeSearchText}&search_simple=1&action=process&json=1&page_size=60";

            HttpResponseMessage? response = null;
            int maxRetries = 6;
            int delayMilliseconds = 2000;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        break;
                    }
                    else
                    {
                        if (i < maxRetries - 1)
                        {
                            await Task.Delay(delayMilliseconds);
                            delayMilliseconds *= 2;
                        }
                    }
                }
                catch (Exception)
                {
                    if (i < maxRetries - 1)
                    {
                        await Task.Delay(delayMilliseconds);
                        delayMilliseconds *= 2;
                    }
                }
            }

            if (response == null || !response.IsSuccessStatusCode)
            {
                return Ok(OFFResults);
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();
            JsonNode? data = JsonNode.Parse(jsonResponse);
            
            if (data == null || data["products"] == null)
            {
                return Ok(OFFResults);
            }

            JsonArray productsArray = data["products"]!.AsArray();
            
            foreach (JsonNode? product in productsArray)
            {
                if (product != null)
                {
                    string openFoodFactsId = product["_id"]?.ToString() ?? "";
                    string name = product["product_name"]?.ToString() ?? "";
                    string brand = product["brands"]?.ToString() ?? "";
                    string imageUrl = product["image_front_small_url"]?.ToString() ?? product["image_url"]?.ToString() ?? "";

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        continue;
                    }

                    string nameAndBrand = name + " " + brand;
                    nameAndBrand = nameAndBrand.ToLower(); 

                    string[] searchWords = searchText.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    bool isValid = true;
                    foreach (string word in searchWords)
                    {
                        if (!nameAndBrand.Contains(word))
                        {
                            isValid = false;
                            break;
                        }
                    }

                    if (!isValid)
                    {
                        continue;
                    }

                    JsonNode? nutriments = product["nutriments"];

                    double protein = 0;
                    double carbs = 0;
                    double fat = 0;
                    double kcal = 0;

                    if (nutriments != null)
                    {
                        if (nutriments["proteins_100g"] != null)
                            double.TryParse(nutriments["proteins_100g"]!.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out protein);

                        if (nutriments["carbohydrates_100g"] != null) 
                            double.TryParse(nutriments["carbohydrates_100g"]!.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out carbs);

                        if (nutriments["fat_100g"] != null) 
                            double.TryParse(nutriments["fat_100g"]!.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out fat);

                        if (nutriments["energy-kcal_100g"] != null) 
                            double.TryParse(nutriments["energy-kcal_100g"]!.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out kcal);
                    }
                    
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

                    bool alreadyExists = false;
                    foreach (Ingredient savedIngredient in OFFResults)
                    {
                        
                        if (savedIngredient.Name.ToLower() == newIngredient.Name.ToLower())
                        {
                            alreadyExists = true;
                            break;
                        }
                    }

                    if (!alreadyExists)
                    {
                        OFFResults.Add(newIngredient);
                    }
                    ;
                }
            }
            client.Dispose();
            return Ok(OFFResults);
        }

        [HttpPost]
        [Route("AddCustomIngredient/{dto}")]
        public async Task<IActionResult> AddCustomIngredient(CustomIngredientDto dto)
        {
            
            if (string.IsNullOrWhiteSpace(dto.Name) || dto.UserId <= 0)
            {
                return BadRequest("El nombre del alimento y el ID del usuario son obligatorios.");
            }

            Ingredient newIngredient = new Ingredient
            {
                Name = dto.Name,
                Brand = dto.Brand,
                Category = "Personalizado",
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
            return Ok();
        }

        [HttpDelete]
        [Route("DeleteCustomIngredient/{id}/{userId}")]
        public async Task<IActionResult> DeleteCustomIngredient(int id,  int userId)
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
    }
}
