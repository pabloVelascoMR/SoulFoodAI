using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;

namespace SoulFoodAiBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoalController : Controller
    {
        private readonly DataContext _context;

        public GoalController(DataContext dataContext)
        {
            _context = dataContext;
        }

        [HttpGet]
        [Route("Goal")]

        public async Task<IActionResult> GetAllGoals()
        {
            List<Goal> goals = await _context.Goals.ToListAsync();

            List<GoalDto> allgoals = goals.

                Select(g => new GoalDto
                {
                    IdGoal = g.IdGoal,
                    GoalName=g.GoalName,
                    Description=g.Description

                }).ToList();

            return Ok(allgoals);
        }

        [HttpPost]
        [Route("Goal")]

        public async Task<IActionResult> AddGoal(CreateGoalDto dto)
        {

            Goal goalAdd = new Goal { GoalName= dto.GoalName , Description= dto.Description};
            await _context.Goals.AddAsync(goalAdd);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        [Route("Goal")]

        public async Task<IActionResult> DeleteGoal(int idGoal)
        {

            Goal? goal = await _context.Goals.FirstOrDefaultAsync(g => g.IdGoal == idGoal);

            if (goal is null) { return NotFound("Ese objetivo no existe."); }

            _context.Goals.Remove(goal);
            await _context.SaveChangesAsync();
            return await GetAllGoals(); ;
        }

        [HttpPut]
        [Route("Goal")]

        public async Task<IActionResult> EditGoal(GoalDto dto)
        { 
            Goal? goalEdit = await _context.Goals.FirstOrDefaultAsync(g => g.IdGoal == dto.IdGoal);

            if (goalEdit is null) { return NotFound("Competicion no existe en la base de datos."); }

            goalEdit.IdGoal= dto.IdGoal;
            goalEdit.GoalName= dto.GoalName;
            goalEdit.Description= dto.Description;

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
