using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;

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

            
            double proteinPercent = 0.25; 
            double carbPercent = 0.45;    
            double fatPercent = 0.30;    

            switch (userData.IdFoodPlan)
            {
                case 1: // Dieta Estándar
                case 2: // Dieta Equilibrada
                case 9: // Dieta Pescetariana
                case 11: // Dieta Baja en FODMAPs
                    proteinPercent = 0.25; carbPercent = 0.45; fatPercent = 0.30;
                    break;
                case 3: 
                    proteinPercent = 0.20; carbPercent = 0.40; fatPercent = 0.40;
                    break;
                case 4: 
                    proteinPercent = 0.20; carbPercent = 0.05; fatPercent = 0.75;
                    break;
                case 5: 
                    proteinPercent = 0.25; carbPercent = 0.10; fatPercent = 0.65;
                    break;
                case 6: 
                    proteinPercent = 0.30; carbPercent = 0.20; fatPercent = 0.50;
                    break;
                case 7: 
                case 12: 
                    proteinPercent = 0.20; carbPercent = 0.55; fatPercent = 0.25;
                    break;
                case 8: 
                    proteinPercent = 0.20; carbPercent = 0.50; fatPercent = 0.30;
                    break;
                case 10: 
                    proteinPercent = 0.20; carbPercent = 0.45; fatPercent = 0.35;
                    break;
            }

            
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
                UserFoodPlanWeekIntolerances = new List<UserFoodPlanWeekIntolerance>()
            };

           
            if (userData.UserIntolerances != null && userData.UserIntolerances.Any())
            {
                foreach (var intol in userData.UserIntolerances)
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
        [Route("GetWeekHeader/{idUser}")]
        public async Task<IActionResult> GetWeekHeader(int idUser)
        {
            
            UserFoodPlanWeek? activeWeek = await _context.UserFoodPlansWeek
                .Include(w => w.FoodPlan)
                .FirstOrDefaultAsync(w => w.IdUser == idUser && w.IsActive);

            if (activeWeek == null)
                return NotFound("No hay plan semanal activo para este usuario.");

     
            var dto = new WeeklyHeaderDto
            {
                IdUserFoodPlanWeek = activeWeek.IdUserFoodPlanWeek,
                DietName = activeWeek.FoodPlan.FoodPlanName,
                TotalWeeklyKcal = activeWeek.TotalWeeklyKcal,
                TargetProteinPercent = activeWeek.FoodPlan.ProteinPercent,
                TargetCarbsPercent = activeWeek.FoodPlan.CarbPercent,
                TargetFatPercent = activeWeek.FoodPlan.FatPercent
            };

            return Ok(dto);
        }
    }
}
