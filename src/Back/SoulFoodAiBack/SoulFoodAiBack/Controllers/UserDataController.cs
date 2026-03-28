using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;

namespace SoulFoodAiBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserDataController : ControllerBase
    {
        private readonly DataContext _context;

        public UserDataController(DataContext dataContext)
        {
            _context = dataContext;
        }

        [HttpGet]
        [Route("UserData")]

        public async Task<IActionResult> GetAlluserDatas()
        {
            List<UserData> userDatas = await _context.UserDatas.ToListAsync();

            List<UserDataDto> allUserDatas = userDatas.

                Select(ud => new UserDataDto
                {
                    IdUserData= ud.IdUserData,
                    Gender = ud.Gender,
                    Age =ud.Age,
                    Height =ud.Height,
                    Weight =ud.Weight,
                    MealsPerDay =ud.MealsPerDay,
                    IdUser =ud.IdUser,
                    IdFoodPlan =ud.IdFoodPlan,
                    IdGoal =ud.IdGoal,
                    IdIntolerance =ud.IdIntolerance,
                 }).ToList();

            return Ok(allUserDatas);
        }
 
    }
}
