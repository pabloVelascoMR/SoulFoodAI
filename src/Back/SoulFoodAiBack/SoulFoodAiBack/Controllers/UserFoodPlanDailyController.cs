using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using System.Globalization;

namespace SoulFoodAiBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserFoodPlanDailyController : ControllerBase
    {
        private readonly DataContext _context;

        public UserFoodPlanDailyController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("GetDailyHeader/{idDailyPlan}")]
        public async Task<IActionResult> GetDailyHeader(int idDailyPlan)
        {
            
            UserFoodPlanDaily? day = await _context.UserFoodPlansDaily
                .Include(d => d.FoodPlan) 
                .Include(d => d.UserFoodPlanWeek)
                .FirstOrDefaultAsync(d => d.IdUserFoodPlanDaily == idDailyPlan);

            if (day == null)
                return NotFound("No se ha encontrado el día especificado.");

            
            CultureInfo culture = new CultureInfo("es-ES");
            string formattedDayName = day.CreationDate.ToString("dddd dd", culture);

            
            formattedDayName = char.ToUpper(formattedDayName[0]) + formattedDayName.Substring(1);


            DailyHeaderDto dto = new DailyHeaderDto
            {
                IdUserFoodPlanDaily = day.IdUserFoodPlanDaily,
                DietName = day.FoodPlan.FoodPlanName,
                DayName = formattedDayName,

                TargetKcal = day.TargetKcal,
                RealKcal = day.RealKcal,

                TargetProtein = Math.Round(day.TargetProtein, 1),
                RealProtein = Math.Round(day.RealProtein, 1),

                TargetCarbs = Math.Round(day.TargetCarbs, 1),
                RealCarbs = Math.Round(day.RealCarbs, 1),

                TargetFat = Math.Round(day.TargetFat, 1),
                RealFat = Math.Round(day.RealFat, 1),

                MealsPerDay = day.UserFoodPlanWeek.MealsPerDay
            };

            return Ok(dto);
        }

        [HttpPost]
        [Route("UpdateDailyRecipes")]
        public async Task<IActionResult> UpdateDailyRecipes([FromBody] UpdateDailyRecipesDto dto)
        {
           
            UserFoodPlanDaily? targetDay = await _context.UserFoodPlansDaily
                .FirstOrDefaultAsync(d => d.IdUserFoodPlanDaily == dto.IdUserFoodPlanDaily);

            if (targetDay == null)
                return NotFound("No se encontró el día especificado.");


            List<FoodPlanDailyRecipe> existingRecipes = await _context.FoodPlanDailyRecipes
                .Where(dr => dr.IdUserFoodPlanDaily == dto.IdUserFoodPlanDaily)
                .ToListAsync();

            if (existingRecipes.Any())
            {
                _context.FoodPlanDailyRecipes.RemoveRange(existingRecipes);
            }

            double totalRealKcal = 0;
            double totalRealProtein = 0;
            double totalRealCarbs = 0;
            double totalRealFat = 0;

            
            foreach (int recipeId in dto.RecipeIds)
            {
                
                Recipe? recipeInfo = await _context.Recipes.FindAsync(recipeId);

                if (recipeInfo != null)
                {
                    
                    await _context.FoodPlanDailyRecipes.AddAsync(new FoodPlanDailyRecipe
                    {
                        IdUserFoodPlanDaily = dto.IdUserFoodPlanDaily,
                        IdRecipe = recipeId
                    });

                   
                    totalRealKcal += recipeInfo.TotalKcal;
                    totalRealProtein += recipeInfo.Protein; 
                    totalRealCarbs += recipeInfo.Carbs;     
                    totalRealFat += recipeInfo.Fat;         
                }
            }

            targetDay.RealKcal = (int)totalRealKcal;
            targetDay.RealProtein = totalRealProtein;
            targetDay.RealCarbs = totalRealCarbs;
            targetDay.RealFat = totalRealFat;

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Día actualizado correctamente con las nuevas recetas." });
        }
    }
}
