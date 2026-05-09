using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using static SoulFoodAiBack.Dtos.WeeklyReportDto;

namespace SoulFoodAiBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserDiaryController : ControllerBase
    {
        private readonly DataContext _context;

        public UserDiaryController(DataContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("SubmitReportAndCreatePlan")]
        public async Task<IActionResult> SubmitReportAndCreatePlan([FromBody] WeeklyReportDto dto)
        {
            try
            {
                var userData = await _context.UserDatas
                    .Include(u => u.FoodPlan)
                    .Include(u => u.UserIntolerances)
                    .FirstOrDefaultAsync(u => u.IdUser == dto.IdUser);

                var activePlan = await _context.UserFoodPlansWeek
                    .FirstOrDefaultAsync(p => p.IdUser == dto.IdUser && p.IsActive);

                if (userData == null)
                    return NotFound(new { message = "Usuario no encontrado." });

                if (activePlan != null)
                {
                    var diaryEntry = new UserDiary
                    {
                        HungerLevel = dto.HungerLevel,
                        EnergyLevel = dto.EnergyLevel,
                        SleepQuality = dto.SleepQuality,
                        DietAdherence = dto.DietAdherence,
                        Description = dto.Description,
                        AiReportResponse = string.Empty, 
                        IdUserFoodPlan = activePlan.IdUserFoodPlanWeek,
                        UserFoodPlan = activePlan,
                        CreationDate = DateTime.Now
                    };
                    await _context.UserDiaries.AddAsync(diaryEntry);
                    activePlan.IsActive = false;
                }

                if (dto.NewWeight.HasValue && dto.NewWeight.Value > 0) userData.Weight = dto.NewWeight.Value;
                if (dto.NewMeasures != null)
                {
                    if (dto.NewMeasures.ChestMeasure > 0) userData.ChestMeasure = dto.NewMeasures.ChestMeasure;
                    if (dto.NewMeasures.WaistMeasure > 0) userData.WaistMeasure = dto.NewMeasures.WaistMeasure;
                    if (dto.NewMeasures.HipMeasure > 0) userData.HipMeasure = dto.NewMeasures.HipMeasure;
                    if (dto.NewMeasures.LeftBicepMeasure > 0) userData.LeftBicepMeasure = dto.NewMeasures.LeftBicepMeasure;
                    if (dto.NewMeasures.RightBicepMeasure > 0) userData.RightBicepMeasure = dto.NewMeasures.RightBicepMeasure;
                    if (dto.NewMeasures.LeftCuadricepsMeasure > 0) userData.LeftCuadricepsMeasure = dto.NewMeasures.LeftCuadricepsMeasure;
                    if (dto.NewMeasures.RightCuadricepsMeasure > 0) userData.RightCuadricepsMeasure = dto.NewMeasures.RightCuadricepsMeasure;
                }

                await _context.SaveChangesAsync();

                return Ok(new { Success = true, Message = "Diario y perfil guardados correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error interno del servidor: {ex.Message}" });
            }
        }
    }
}