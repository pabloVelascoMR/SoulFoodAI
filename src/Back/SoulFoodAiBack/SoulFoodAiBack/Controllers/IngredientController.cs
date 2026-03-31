using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Models;

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
        [Route("Ingredient/{category}")]
        public async Task<ActionResult<List<Ingredient>>> GetIngredients(string category)
        {
            List<Ingredient> ingredients = await _context.Ingredients
                       .Where(i => i.Category.ToLower() == category.ToLower())
                       .ToListAsync();

            return Ok(ingredients);
        }

        [HttpGet]
        [Route("Ingredient")]
        public async Task<ActionResult<List<Ingredient>>> GetAllIngredients()
        {
            List<Ingredient> ingredients = await _context.Ingredients.ToListAsync();
            return Ok(ingredients);
        }
    }
}
