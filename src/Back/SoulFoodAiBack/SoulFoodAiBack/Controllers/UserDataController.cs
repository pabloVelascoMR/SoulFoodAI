using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using System.Security.Claims;

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

        [HttpPost]
        [Route("UserData")]

        public async Task<IActionResult> AddUserData(CreateUserDataDto dto)
        {
            

            UserData userDataAdd = new UserData 
            {
                Gender = dto.Gender,
                Age = dto.Age,
                Height = dto.Height,
                Weight = dto.Weight,
                MealsPerDay = dto.MealsPerDay > 0 ? dto.MealsPerDay : 3,
                IdUser = dto.IdUser,
                IdFoodPlan = dto.IdFoodPlan > 0 ? dto.IdFoodPlan : 2,
                IdGoal = dto.IdGoal > 0 ? dto.IdGoal : 6,
                IdIntolerance = dto.IdIntolerance > 0 ? dto.IdIntolerance : 12
            };

            await _context.UserDatas.AddAsync(userDataAdd);
            await _context.SaveChangesAsync();
            return Ok();
        }

        
    }
}
