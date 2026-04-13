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
    }
}
