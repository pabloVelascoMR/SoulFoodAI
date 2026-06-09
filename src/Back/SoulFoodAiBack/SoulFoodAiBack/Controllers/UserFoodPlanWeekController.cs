using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using System.Globalization;

namespace SoulFoodAiBack.Controllers
{
    /// <summary>
    /// Controlador encargado de la generación, orquestación y consulta de los planes nutricionales semanales de los usuarios.
    /// Implementa los algoritmos basales de cálculo metabólico (TMB) para la asignación de metas calóricas y macros.
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
    public class UserFoodPlanWeekController : ControllerBase
    {
        /// <summary>
        /// Contexto transaccional de Entity Framework para la interacción con la base de datos relacional.
        /// </summary>
        private readonly DataContext _context;

        /// <summary>
        /// Constructor de la clase que inyecta la dependencia requerida del contexto de base de datos.
        /// </summary>
        /// <param name="context">Instancia del contexto de Entity Framework.</param>
        public UserFoodPlanWeekController(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Calcula y estructura un nuevo plan de alimentación semanal tomando como base la antropometría y el objetivo del usuario.
        /// Aplica la ecuación revisada de Mifflin-St Jeor para estimar la Tasa Metabólica Basal (BMR) y establece 
        /// la estructura temporal de la semana junto a sus respectivos requerimientos calóricos diarios.
        /// </summary>
        /// <param name="idUser">Identificador del usuario demandante de la generación del plan.</param>
        /// <returns>Código 200 (OK) incluyendo el identificador unívoco de la semana de planificación generada.</returns>
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
            if (userData?.FoodPlan == null || userData.Goal == null)
                return NotFound("Faltan datos del usuario. Debe completar el Onboarding.");
            double bmr = userData.Gender == "Hombre"
                ? (10 * userData.Weight) + (6.25 * (userData.Height * 100)) - (5 * userData.Age) + 5
                : (10 * userData.Weight) + (6.25 * (userData.Height * 100)) - (5 * userData.Age) - 161;
            double tdee = bmr * 1.3;
            double dailyKcalTarget = tdee;
            switch (userData.IdGoal)
            {
                case 1: dailyKcalTarget -= 400; break;
                case 2: dailyKcalTarget += 300; break;
                case 3: break;
                case 4: dailyKcalTarget -= 200; break;
                case 5: dailyKcalTarget += 250; break;
                case 6: break;
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
            if (userData.UserIntolerances != null && userData.UserIntolerances.Count > 0)
            {
                foreach (UserIntolerance? intol in userData.UserIntolerances)
                {
                    if (intol != null)
                    {
                        newWeek.UserFoodPlanWeekIntolerances.Add(new UserFoodPlanWeekIntolerance
                        {
                            IdIntolerance = intol.IdIntolerance
                        });
                    }
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
                Message = "Plan Semanal y 7 d as generados correctamente.",
                IdUserFoodPlanWeek = newWeek.IdUserFoodPlanWeek
            });
        }

        /// <summary>
        /// Genera una vista agregada y resumida de las métricas acumuladas correspondientes a la semana en curso del usuario.
        /// </summary>
        /// <param name="idUser">Identificador del perfil del usuario.</param>
        /// <returns>Objeto estructurado con los sumatorios totales de calorías y macronutrientes semanales.</returns>
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
                DietName = activeWeek.FoodPlan?.FoodPlanName ?? string.Empty,
                TotalWeeklyKcal = activeWeek.TotalWeeklyKcal,
                TargetProteinPercent = activeWeek.TotalWeeklyKcal > 0 ? Math.Round((activeWeek.TotalWeeklyProtein * 4 / activeWeek.TotalWeeklyKcal) * 100) : 0,
                TargetCarbsPercent = activeWeek.TotalWeeklyKcal > 0 ? Math.Round((activeWeek.TotalWeeklyCarbs * 4 / activeWeek.TotalWeeklyKcal) * 100) : 0,
                TargetFatPercent = activeWeek.TotalWeeklyKcal > 0 ? Math.Round((activeWeek.TotalWeeklyFat * 9 / activeWeek.TotalWeeklyKcal) * 100) : 0
            };
            return Ok(dto);
        }

        /// <summary>
        /// Consulta y estructura el plan semanal en una jerarquía profunda de datos (Días -> Recetas -> Ingredientes)
        /// facilitando la renderización de la vista pormenorizada de calendario en el cliente.
        /// </summary>
        /// <param name="idUser">Identificador único del usuario propietario de la semana en curso.</param>
        /// <returns>Colección jerárquica con los planes diarios detallados del usuario.</returns>
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
                .Include(d => d.FoodPlanDailyRecipes)
                    .ThenInclude(dr => dr.Recipe)
                        .ThenInclude(r => r.RecipeUserIngredients)
                            .ThenInclude(ri => ri.Ingredient)
                .Where(d => d.IdUserFoodPlanWeek == activeWeek.IdUserFoodPlanWeek)
                .OrderBy(d => d.CreationDate)
                .ToListAsync();
            var culture = new CultureInfo("es-ES");
            var calendarDto = new WeekCalendarDto
            {
                IdUserFoodPlanWeek = activeWeek.IdUserFoodPlanWeek,
                MealsPerDay = activeWeek.MealsPerDay,
                Days = weekDays.Select(day => {
                    string dayStr = day.CreationDate.ToString("dddd", culture);
                    string finalDayName = string.IsNullOrEmpty(dayStr) ? string.Empty : char.ToUpper(dayStr[0]) + dayStr.Substring(1);
                    return new DayCalendarDto
                    {
                        IdUserFoodPlanDaily = day.IdUserFoodPlanDaily,
                        DayName = finalDayName,
                        DateNumber = day.CreationDate.ToString("dd"),
                        FullDate = day.CreationDate,
                        AssignedRecipes = day.FoodPlanDailyRecipes.Select(dr => new DailyRecipeDto
                        {
                            IdRecipe = dr.IdRecipe,
                            RecipeName = dr.Recipe?.RecipeName ?? string.Empty,
                            Kcal = dr.Recipe?.TotalKcal ?? 0,
                            MealType = dr.Recipe?.Meal?.MealName ?? string.Empty,
                            Protein = dr.Recipe?.Protein ?? 0,
                            Carbs = dr.Recipe?.Carbs ?? 0,
                            Fat = dr.Recipe?.Fat ?? 0,
                            RecipeDescription = dr.Recipe?.RecipeDescription,
                            Ingredients = dr.Recipe?.RecipeUserIngredients.Select(ri => new RecipeIngredientDetailDto
                            {
                                Name = ri.Ingredient?.Name ?? string.Empty,
                                Quantity = ri.Quantity,
                                Unit = ri.Unit
                            }).ToList() ?? new List<RecipeIngredientDetailDto>()
                        }).ToList()
                    };
                }).ToList()
            };
            return Ok(calendarDto);
        }

        /// <summary>
        /// Recupera el conjunto de planificaciones semanales históricas que el usuario ha finalizado y cerrado, 
        /// proporcionando trazabilidad y registro de su progreso dietético anterior.
        /// </summary>
        /// <param name="idUser">Identificador primario del usuario consultado.</param>
        /// <returns>Colección de registros de las planificaciones semanales archivadas.</returns>
        [HttpGet]
        [Route("GetPlanHistory/{idUser}")]
        public async Task<IActionResult> GetPlanHistory(int idUser)
        {
            var history = await _context.UserFoodPlansWeek
                .Where(w => w.IdUser == idUser && w.IsVisibleInHistory)
                .Include(w => w.FoodPlan)
                .Include(w => w.UserFoodPlanMeals)
                    .ThenInclude(d => d.FoodPlanDailyRecipes)
                        .ThenInclude(dr => dr.Recipe)
                            .ThenInclude(r => r.Meal)
                .OrderByDescending(w => w.StartDate)
                .ToListAsync();
            if (history == null)
                return NotFound("Historial no encontrado.");
            var filteredHistory = history
                .Where(w => w.UserFoodPlanMeals.Count > 0 && w.UserFoodPlanMeals.Any(d => d.FoodPlanDailyRecipes.Count > 0))
                .Select(w => new {
                    w.IdUserFoodPlanWeek,
                    DietName = w.FoodPlan?.FoodPlanName ?? "Plan Personalizado",
                    w.StartDate,
                    w.EndDate,
                    w.IsActive,
                    RecipesEaten = w.UserFoodPlanMeals
                        .OrderBy(d => d.IdUserFoodPlanDaily)
                        .SelectMany((d, index) => d.FoodPlanDailyRecipes.Select(dr => new {
                            IdRecipe = dr.Recipe?.IdRecipe,
                            RecipeName = dr.Recipe?.RecipeName ?? string.Empty,
                            MealType = dr.Recipe?.Meal?.MealName ?? string.Empty,
                            DateEaten = w.StartDate.AddDays(index)
                        }))
                    .ToList()
                })
                .ToList();
            if (filteredHistory.Count == 0)
                return NotFound("No se encontraron planes con recetas en tu historial.");
            return Ok(filteredHistory);
        }

        /// <summary>
        /// Aplica un marcado de borrado lógico (archivado) sobre una planificación semanal completada,
        /// impidiendo su recuperación en las consultas del historial desde la interfaz sin destruir la integridad física de los datos.
        /// </summary>
        /// <param name="idUserFoodPlanWeek">Identificador de la semana específica que se desea ocultar.</param>
        /// <returns>Confirmación exitosa de la actualización de la bandera de estado lógica.</returns>
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