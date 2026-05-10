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
    public class UserFoodPlanWeekController : ControllerBase
    {

        private readonly DataContext _context;

        public UserFoodPlanWeekController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("GenerateWeekPlan/{idUser}")]
        public async Task<IActionResult> GenerateWeekPlan(int idUser)
        {
            
            UserFoodPlanWeek? activePlan = await _context.UserFoodPlansWeek
                .FirstOrDefaultAsync(w => w.IdUser == idUser && w.IsActive);

            if (activePlan != null)
                return BadRequest("El usuario ya tiene un plan semanal activo.");

           
            UserData? userData = await _context.UserDatas
                .Include(u => u.FoodPlan)
                .Include(u => u.Goal)
                .Include(u => u.UserIntolerances)
                .FirstOrDefaultAsync(u => u.IdUser == idUser);

            if (userData == null)
                return NotFound("Faltan datos del usuario. Debe completar el Onboarding.");


            
            double bmr = userData.Gender == "Hombre"
                ? (10 * userData.Weight) + (6.25 * (userData.Height * 100)) - (5 * userData.Age) + 5
                : (10 * userData.Weight) + (6.25 * (userData.Height * 100)) - (5 * userData.Age) - 161;

            double tdee = bmr * 1.3;

            double dailyKcalTarget = tdee;
            switch (userData.IdGoal)
            {
                case 1: // Pérdida de grasa 
                    dailyKcalTarget -= 400;
                    break;
                case 2: // Ganancia muscular 
                    dailyKcalTarget += 300;
                    break;
                case 3: // Mantenimiento Saludable
                    break;
                case 4: // Recomposición corporal
                    dailyKcalTarget -= 200;
                    break;
                case 5: // Rendimiento deportivo
                    dailyKcalTarget += 250;
                    break;
                case 6: // Sin objetivo específico
                    break;
            }

            int finalDailyKcal = (int)Math.Round(dailyKcalTarget);

            if (finalDailyKcal < 1200) finalDailyKcal = 1200;

            
            double proteinPercent = userData.FoodPlan.ProteinPercent; 
            double carbPercent = userData.FoodPlan.CarbPercent;    
            double fatPercent = userData.FoodPlan.FatPercent;    
            
            double dailyProteinGrams = (finalDailyKcal * proteinPercent) / 4.0;
            double dailyCarbsGrams = (finalDailyKcal * carbPercent) / 4.0;
            double dailyFatGrams = (finalDailyKcal * fatPercent) / 9.0;

           
            DateTime today = DateTime.Today;
            UserFoodPlanWeek newWeek = new UserFoodPlanWeek
            {
                IdUser = idUser,
                IdFoodPlan = userData.IdFoodPlan,
                IdGoal = userData.IdGoal,
                StartDate = today,
                EndDate = today.AddDays(6), 
                MealsPerDay = userData.MealsPerDay,
                TotalWeeklyKcal = finalDailyKcal * 7,
                TotalWeeklyProtein = dailyProteinGrams * 7,
                TotalWeeklyCarbs = dailyCarbsGrams * 7,
                TotalWeeklyFat = dailyFatGrams * 7,
                IsActive = true,
                IsVisibleInHistory = true,
                UserFoodPlanWeekIntolerances = new List<UserFoodPlanWeekIntolerance>()
            };

           
            if (userData.UserIntolerances != null && userData.UserIntolerances.Any())
            {
                foreach (UserIntolerance? intol in userData.UserIntolerances)
                {
                    newWeek.UserFoodPlanWeekIntolerances.Add(new UserFoodPlanWeekIntolerance
                    {
                        IdIntolerance = intol.IdIntolerance
                    });
                }
            }

            await _context.UserFoodPlansWeek.AddAsync(newWeek);
            await _context.SaveChangesAsync(); 

            for (int i = 0; i < 7; i++)
            {
                UserFoodPlanDaily newDay = new UserFoodPlanDaily
                {
                    IdUser = idUser,
                    IdUserFoodPlanWeek = newWeek.IdUserFoodPlanWeek,
                    IdFoodPlan = userData.IdFoodPlan,
                    TargetKcal = finalDailyKcal,
                    TargetProtein = dailyProteinGrams,
                    TargetCarbs = dailyCarbsGrams,
                    TargetFat = dailyFatGrams,
                    RealKcal = 0,
                    RealProtein = 0,
                    RealCarbs = 0,
                    RealFat = 0,
                    IsActive = true,
                    CreationDate = today.AddDays(i)
                };
                await _context.UserFoodPlansDaily.AddAsync(newDay);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Plan Semanal y 7 días generados correctamente.",
                IdUserFoodPlanWeek = newWeek.IdUserFoodPlanWeek
            });
        }

        [HttpGet]
        [Route("GetWeeklyHeader/{idUser}")]
        public async Task<IActionResult> GetWeeklyHeader(int idUser)
        {
            var activeWeek = await _context.UserFoodPlansWeek
                .Include(w => w.FoodPlan)
                .FirstOrDefaultAsync(w => w.IdUser == idUser && w.IsActive);

            if (activeWeek == null)
                return NotFound("No hay plan semanal activo para este usuario.");

            var dto = new WeeklyHeaderDto
            {
                IdUserFoodPlanWeek = activeWeek.IdUserFoodPlanWeek,
                DietName = activeWeek.FoodPlan.FoodPlanName,
                TotalWeeklyKcal = activeWeek.TotalWeeklyKcal,
                TargetProteinPercent = activeWeek.TotalWeeklyKcal > 0 ? Math.Round((activeWeek.TotalWeeklyProtein * 4 / activeWeek.TotalWeeklyKcal) * 100) : 0,
                TargetCarbsPercent = activeWeek.TotalWeeklyKcal > 0 ? Math.Round((activeWeek.TotalWeeklyCarbs * 4 / activeWeek.TotalWeeklyKcal) * 100) : 0,
                TargetFatPercent = activeWeek.TotalWeeklyKcal > 0 ? Math.Round((activeWeek.TotalWeeklyFat * 9 / activeWeek.TotalWeeklyKcal) * 100) : 0
            };

            return Ok(dto);
        }

        [HttpGet]
        [Route("GetActiveWeekCalendar/{idUser}")]
        public async Task<IActionResult> GetActiveWeekCalendar(int idUser)
        {
            var activeWeek = await _context.UserFoodPlansWeek
                .FirstOrDefaultAsync(w => w.IdUser == idUser && w.IsActive);

            if (activeWeek == null)
                return NotFound("No hay plan semanal activo para este usuario.");

            var weekDays = await _context.UserFoodPlansDaily
                .Include(d => d.FoodPlanDailyRecipes) 
                    .ThenInclude(dr => dr.Recipe)         
                        .ThenInclude(r => r.Meal)          
                .Where(d => d.IdUserFoodPlanWeek == activeWeek.IdUserFoodPlanWeek)
                .OrderBy(d => d.CreationDate)              
                .ToListAsync();

            
            var culture = new System.Globalization.CultureInfo("es-ES");

            
            var calendarDto = new WeekCalendarDto
            {
                IdUserFoodPlanWeek = activeWeek.IdUserFoodPlanWeek,
                MealsPerDay = activeWeek.MealsPerDay, 

                Days = weekDays.Select(day => new DayCalendarDto
                {
                    IdUserFoodPlanDaily = day.IdUserFoodPlanDaily,

                    DayName = char.ToUpper(day.CreationDate.ToString("dddd", culture)[0]) +
                              day.CreationDate.ToString("dddd", culture).Substring(1),

                    DateNumber = day.CreationDate.ToString("dd"),

                    FullDate = day.CreationDate,

                    AssignedRecipes = day.FoodPlanDailyRecipes.Select(dr => new DailyRecipeDto
                    {
                        IdRecipe = dr.IdRecipe,
                        RecipeName = dr.Recipe.RecipeName,
                        Kcal = dr.Recipe.TotalKcal,
                        MealType = dr.Recipe.Meal.MealName 
                    }).ToList()
                }).ToList()
            };

            return Ok(calendarDto);
        }

        [HttpGet]
        [Route("GetPlanHistory/{idUser}")]
        public async Task<IActionResult> GetPlanHistory(int idUser)
        {
            List<UserFoodPlanWeek>? history = await _context.UserFoodPlansWeek
                .Where(w => w.IdUser == idUser && w.IsVisibleInHistory) 
                .Include(w => w.FoodPlan)
                .Include(w => w.UserFoodPlanMeals) 
                    .ThenInclude(d => d.FoodPlanDailyRecipes) 
                        .ThenInclude(dr => dr.Recipe)
                            .ThenInclude(r => r.Meal)
                .OrderByDescending(w => w.StartDate)
                .ToListAsync();

            var filteredHistory = history
                .Where(w => w.UserFoodPlanMeals.Any(d => d.FoodPlanDailyRecipes.Any())) 
                .Select(w => new {
                    w.IdUserFoodPlanWeek,
                    DietName = w.FoodPlan?.FoodPlanName ?? "Plan Personalizado",
                    w.StartDate,
                    w.EndDate,
                    w.IsActive,
                    RecipesEaten = w.UserFoodPlanMeals
                        .OrderBy(d => d.IdUserFoodPlanDaily) 
                        .SelectMany((d, index) => d.FoodPlanDailyRecipes.Select(dr => new {
                        dr.Recipe.IdRecipe,
                        dr.Recipe.RecipeName,
                        MealType = dr.Recipe.Meal?.MealName,                  
                        DateEaten = w.StartDate.AddDays(index)
                    }))
                    .ToList()
                })
                .ToList();

            if (!filteredHistory.Any())
                return NotFound("No se encontraron planes con recetas en tu historial.");

            return Ok(filteredHistory);
        }

        [HttpPut]
        [Route("HidePlanFromHistory/{idUserFoodPlanWeek}")]
        public async Task<IActionResult> HidePlanFromHistory(int idUserFoodPlanWeek)
        {
            UserFoodPlanWeek? plan = await _context.UserFoodPlansWeek.FindAsync(idUserFoodPlanWeek);

            if (plan == null)
                return NotFound("El plan especificado no existe.");

            plan.IsVisibleInHistory = false;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Plan ocultado del historial correctamente." });
        }
    }
}
