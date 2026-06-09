using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;

namespace SoulFoodAiBack.Controllers
{
    /// <summary>
    /// Controlador responsable de gestionar el subconjunto de ingredientes explícitamente permitidos o favoritos de un usuario, 
    /// conformando la base de conocimiento dietético exclusiva a partir de la cual el motor de IA compondrá las recetas personalizadas.
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
    public class UserIngredientController : ControllerBase
    {
        /// <summary>
        /// Contexto inyectado de la base de datos transaccional proporcionada por Entity Framework.
        /// </summary>
        private readonly DataContext _context;

        /// <summary>
        /// Constructor de la clase que provee la inyección de la dependencia del contexto relacional de base de datos.
        /// </summary>
        /// <param name="context">Instancia del contexto de Entity Framework.</param>
        public UserIngredientController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Establece la relación directa N:M entre un usuario y un ingrediente, agregándolo de este modo a su catálogo de favoritos, 
        /// admitiendo para ello tanto referencias del inventario local como peticiones de inclusión basadas en repositorios externos.
        /// </summary>
        /// <param name="dto">Objeto de transferencia de datos con el mapeo del ingrediente deseado.</param>
        /// <returns>Mensaje de confirmación del éxito de la transacción.</returns>
        [HttpPost]
        [Route("AddFavorite")]
        public async Task<IActionResult> AddFavorite(SaveFavouriteIngredientDto dto)
        {
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

        /// <summary>
        /// Desvincula físicamente un ingrediente de la lista restrictiva de favoritos del usuario al eliminar la tabla asociativa intermedia.
        /// </summary>
        /// <param name="Idingredient">Identificador del ingrediente objetivo.</param>
        /// <param name="IdUser">Identificador principal del usuario propietario de la lista.</param>
        /// <returns>Mensaje que notifica la correcta destrucción de la relación lógica.</returns>
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

        /// <summary>
        /// Consulta y proyecta la colección completa de ingredientes específicos que el usuario ha habilitado y seleccionado para su consumo.
        /// </summary>
        /// <param name="userId">Identificador primario del usuario sometido a consulta.</param>
        /// <returns>Colección estructurada de ingredientes autorizados encapsulada en formato DTO.</returns>
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
                    IdIngredient = ui.Ingredient!.IdIngredient,
                    Category = ui.Ingredient!.Category
                })
                .ToListAsync();

            return Ok(favorites);
        }
    }
}