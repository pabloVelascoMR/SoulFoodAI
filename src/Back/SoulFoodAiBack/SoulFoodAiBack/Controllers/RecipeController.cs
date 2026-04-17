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
            List<RecipeCardDto>? availableRecipes = await _context.FoodPlanDailyRecipes
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


        [HttpPost]
        [Route("AddRecipesForUser/{idUser}")]
        public async Task<IActionResult> AddRecipesForUser (int idUser, AddRecipeDto dto)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.IdUser == idUser);
            if (user is null) { return NotFound("Ese usuario no existe."); }

            Recipe? recipe = _context.Recipes
                                .Where(r=>r.RecipeName == dto.RecipeName)
                                .Where(r => r.IdUser== idUser)
                                .FirstOrDefault();

            if (recipe != null) { return BadRequest("Esta receta con este nombre ya existe."); }

            Recipe recipeAdd = new Recipe
             {
                IdMeal = dto.IdMeal,
                RecipeName = dto.RecipeName,    
                IdUser=idUser,
                RecipeDescription = dto.RecipeDescription,   
              };

            _context.Recipes.Add(recipeAdd);
            await _context.SaveChangesAsync();

            Recipe? recipeSaved =  _context.Recipes
                                .Where(r => r.RecipeName == dto.RecipeName)
                                .Where(r => r.IdUser == idUser)
                                .FirstOrDefault();
           
            int i = 0;
            foreach (int IdIngredient in dto.IdIngredients)
            {
                RecipeUserIngredient recipeIngredient = new RecipeUserIngredient
                {
                    IdRecipe = recipeAdd.IdRecipe,
                    IdIngredient = IdIngredient,
                    Quantity = dto.Quantity.ElementAt(i),
                    Unit = dto.Unit.ElementAt(i) 
                };
                _context.RecipeUserIngredients.Add(recipeIngredient);
                i++;
            }
            await _context.SaveChangesAsync();

            List<RecipeUserIngredient>? ingredientesDeLaReceta = await _context.RecipeUserIngredients
                                                .Include(ri => ri.Ingredient)
                                                .Where(ri => ri.IdRecipe == recipeAdd.IdRecipe)
                                                .ToListAsync();
            recipeAdd.Protein = ingredientesDeLaReceta.Sum(ri => (ri.Quantity * ri.Ingredient.Protein) / 100);
            recipeAdd.Carbs = ingredientesDeLaReceta.Sum(ri => (ri.Quantity * ri.Ingredient.Carbs) / 100);
            recipeAdd.Fat = ingredientesDeLaReceta.Sum(ri => (ri.Quantity * ri.Ingredient.Fat) / 100);
            recipeAdd.TotalKcal = (int)ingredientesDeLaReceta.Sum(ri => (ri.Quantity * ri.Ingredient.Kcal) / 100);

            await _context.SaveChangesAsync();

            return Ok();
        } 
    }
}
