using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;

namespace SoulFoodAiBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeController : ControllerBase
    {
        private readonly DataContext _context;

        public RecipeController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetRecipesForUser/{idUser}")]
        public async Task<IActionResult> GetRecipesForUser(int idUser)
        {
            var availableRecipes = await _context.FoodPlanDailyRecipes
                .Include(r => r.Recipe)
                    .ThenInclude(recipe => recipe.Meal) 
                .Include(r => r.User)
                    .ThenInclude(u => u.UserData)
                        .ThenInclude(ud => ud.FoodPlan) 
                .Where(r => r.IdUser == idUser)
                .Select(r => new RecipeCardDto
                {
                    IdRecipe = r.IdRecipe,
                    RecipeName = r.Recipe.RecipeName,
                    Kcal = r.Recipe.TotalKcal, 
                    MealName = r.Recipe.Meal.MealName,
                    DietName = r.User.UserData.FoodPlan.FoodPlanName
                })
                .ToListAsync();

            if (availableRecipes == null || !availableRecipes.Any())
            {
                return Ok(new List<RecipeCardDto>());
            }

            return Ok(availableRecipes);
        }

    }
}
