using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;

namespace SoulFoodAiBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MealController : ControllerBase
    {
        private readonly DataContext _context;

        public MealController(DataContext dataContext)
        {
            _context = dataContext;
        }

        [HttpGet]
        [Route("Meal")]

        public async Task<IActionResult> GetAllMeals()
        {
            List<Meal> meals = await _context.Meals.ToListAsync();

            List<MealDto> allmeals = meals.

                Select(m => new MealDto
                {
                    IdMeal = m.IdMeal,
                    MealName = m.MealName,  

                }).ToList();

            return Ok(allmeals);
        }

        [HttpPost]
        [Route("Meal")]

        public async Task<IActionResult> AddMeal(CreateMealDto dto)
        {

            Meal mealAdd = new Meal { MealName = dto.MealName};
            await _context.Meals.AddAsync(mealAdd);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        [Route("Meal")]

        public async Task<IActionResult> DeleteMeal(int idMeal)
        {

            Meal? meal = await _context.Meals.FirstOrDefaultAsync(m => m.IdMeal == idMeal);

            if (meal is null) { return NotFound("Esa comida no existe."); }

            _context.Meals.Remove(meal);
            await _context.SaveChangesAsync();
            return await GetAllMeals(); ;
        }

        [HttpPut]
        [Route("Meal")]

        public async Task<IActionResult> EditMeal(MealDto dto)
        {
            Meal? mealEdit = await _context.Meals.FirstOrDefaultAsync(m => m.IdMeal == dto.IdMeal);

            if (mealEdit is null) { return NotFound("Comida no existe en la base de datos."); }

            mealEdit.IdMeal = dto.IdMeal;
            mealEdit.MealName = dto.MealName;
            
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
