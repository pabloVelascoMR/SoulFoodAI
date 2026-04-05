using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;

namespace SoulFoodAiBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserIngredientController : ControllerBase
    {
        private readonly DataContext _context;

        public UserIngredientController(DataContext context)
        {
            _context = context;
        }
        
        [HttpPost]
        [Route("addFavorite")]
        public async Task<IActionResult> AddFavorite(SaveFavouriteIngredientDto dto)
        {
            if (dto.IdUser <= 0)
            {
                return BadRequest("El ID de usuario no es valido.");
            }

            Ingredient? ingredientSave = null;

            if (dto.LocalIdIngredient.HasValue && dto.LocalIdIngredient > 0)
            {
                ingredientSave = await _context.Ingredients.FindAsync(dto.LocalIdIngredient);
            }
            
            else if (!string.IsNullOrEmpty(dto.IdOpenFoodFacts))
            {
                
                ingredientSave = await _context.Ingredients
                    .FirstOrDefaultAsync(i => i.OpenFoodFactsId == dto.IdOpenFoodFacts);

                if (ingredientSave == null)
                {
                    if (string.IsNullOrWhiteSpace(dto.Name))
                    {
                        return BadRequest("Faltan datos para importar el alimento.");
                    }

                    ingredientSave = new Ingredient
                    {
                        Name = dto.Name,
                        Brand = dto.Brand,
                        ImageUrl = dto.ImageUrl,
                        OpenFoodFactsId = dto.IdOpenFoodFacts,
                        Category = "", 
                        Protein = dto.Protein,
                        Carbs = dto.Carbs,
                        Fat = dto.Fat,
                        Kcal = dto.Kcal
                    };

                    _context.Ingredients.Add(ingredientSave);
                    await _context.SaveChangesAsync(); 
                }
            }

            if (ingredientSave == null)
            {
                return BadRequest("No se pudo identificar ni procesar el ingrediente.");
            }

            bool alreadyExists = await _context.UserIngredients
                .AnyAsync(ui => ui.IdUser == dto.IdUser && ui.IdIngredient == ingredientSave.IdIngredient);

            if (alreadyExists)
            {
                return Ok(new { message = "Este ingrediente ya estaba en tu lista." });
            }

            
            UserIngredient newFavorite = new UserIngredient
            {
                IdUser = dto.IdUser,
                IdIngredient = ingredientSave.IdIngredient
            };

            _context.UserIngredients.Add(newFavorite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Ingrediente añadido con éxito." });
        }
    }
}
