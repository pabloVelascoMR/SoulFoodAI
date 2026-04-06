using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;

namespace SoulFoodAiBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodPlanController : ControllerBase
    {
        private readonly DataContext _context;

        public FoodPlanController(DataContext dataContext)
        {
            _context = dataContext;
        }

        [HttpGet]
        [Route("GetAllFoodPlan")]

        public async Task<IActionResult> GetAllFoodPlan()
        {
            List<FoodPlan> foodPlans = await _context.FoodPlans.ToListAsync();

            List<FoodPlanDto> allfoodPlan = foodPlans.

                Select(fp => new FoodPlanDto
                {
                    IdFoodPlan = fp.IdFoodPlan,
                    FoodPlanName = fp.FoodPlanName,
                    Description = fp.Description

                }).ToList();

            return Ok(allfoodPlan);
        }

        [HttpPost]
        [Route("AddFoodPlan")]

        public async Task<IActionResult> AddFoodPlan(CreateFoodPlanDto dto)
        {

            FoodPlan foodPlanAdd = new FoodPlan { FoodPlanName = dto.FoodPlanName, Description = dto.Description };
            await _context.FoodPlans.AddAsync(foodPlanAdd);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        [Route("DeleteFoodPlan")]

        public async Task<IActionResult> DeleteFoodPlan(int idFoodPlan)
        {

            FoodPlan? foodPlan = await _context.FoodPlans.FirstOrDefaultAsync(fp => fp.IdFoodPlan == idFoodPlan);

            if (foodPlan is null) { return NotFound("Ese plan no existe."); }

            _context.FoodPlans.Remove(foodPlan);
            await _context.SaveChangesAsync();
            return await GetAllFoodPlan(); ;
        }

        [HttpPut]
        [Route("EditFoodPlan")]

        public async Task<IActionResult> EditFoodPlan(FoodPlanDto dto)
        {
            FoodPlan? foodPlanEdit = await _context.FoodPlans.FirstOrDefaultAsync(fp => fp.IdFoodPlan == dto.IdFoodPlan);

            if (foodPlanEdit is null) { return NotFound("Plan no existe en la base de datos."); }

            foodPlanEdit.IdFoodPlan = dto.IdFoodPlan;
            foodPlanEdit.FoodPlanName = dto.FoodPlanName;
            foodPlanEdit.Description = dto.Description;

            await _context.SaveChangesAsync();
            return Ok();
        }


    }
}

