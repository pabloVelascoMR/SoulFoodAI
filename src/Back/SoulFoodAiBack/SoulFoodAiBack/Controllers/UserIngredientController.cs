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
        [Route("AddFavorite")]
        public async Task<IActionResult> AddFavorite(SaveFavouriteIngredientDto dto)
        {
            if (dto.IdUser <= 0)
            {
                return BadRequest("El ID de usuario no es valido.");
            }

            if (dto.IdUser <= 0)
            {
                return BadRequest("El ID de usuario no es valido.");
            }

            bool userExists = await _context.Users.AnyAsync(u => u.IdUser == dto.IdUser);
            if (!userExists)
            {
                return NotFound($"No se puede guardar el favorito porque el usuario con ID {dto.IdUser} no existe en la base de datos.");
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

        [HttpDelete]
        [Route("RemoveFavorite/{Idingredient}/{IdUser}")]
        public async Task<IActionResult> RemoveFavorite(int Idingredient, int IdUser)
        {

            if (IdUser <= 0 || Idingredient <= 0)
            {
                return BadRequest("Datos inválidos para eliminar el favorito.");
            }

            UserIngredient? favorite = await _context.UserIngredients
                .FirstOrDefaultAsync(ui => ui.IdUser == IdUser && ui.IdIngredient == Idingredient);

            if (favorite == null)
            {
                return NotFound("El ingrediente no está en tu lista de favoritos.");
            }

            _context.UserIngredients.Remove(favorite);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Ingrediente eliminado de tu lista de preferencias." });
        }

        [HttpGet]
        [Route("GetFavorites/{userId}")]
        public async Task<IActionResult> GetFavorites(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Usuario no válido.");
            }

            List<IngredientsFavoritesDto> favorites = await _context.UserIngredients.AsNoTracking()
                .Where(ui => ui.IdUser == userId)
                .Select(ui => new IngredientsFavoritesDto
                {
                    IdIngredient = ui.Ingredient.IdIngredient,
                    Category = ui.Ingredient.Category
                })
                .ToListAsync();

            return Ok(favorites);
        }
    }
}
