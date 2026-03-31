using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using System.Reflection;
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

        public async Task<IActionResult> GetAllUserDatas()
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

        [HttpDelete]
        [Route("UserData")]
        public async Task<IActionResult> DeleteUserData(int idUser)
        {

            UserData? userData = await _context.UserDatas.FirstOrDefaultAsync(u => u.IdUser == idUser);

            if (userData is null) { return NotFound("Ese usaurio no existe."); }

            _context.UserDatas.Remove(userData);
            await _context.SaveChangesAsync();
            return await GetAllUserDatas(); ;
        }

        [HttpPut]
        [Route("UserData")]

        public async Task<IActionResult> EditUserData(UserDataDto dto)
        {
            UserData? userDataEdit = await _context.UserDatas.FirstOrDefaultAsync(u => u.IdUser == dto.IdUser);

            if (userDataEdit is null) { return NotFound("Objetivo no existe en la base de datos."); }

            userDataEdit.IdGoal = dto.IdGoal;
            userDataEdit.Gender = dto.Gender;
            userDataEdit.Age = dto.Age;
            userDataEdit.Height = dto.Height;
            userDataEdit.Weight = dto.Weight;
            userDataEdit.MealsPerDay = dto.MealsPerDay;
            userDataEdit.IdUser = dto.IdUser;
            userDataEdit.IdFoodPlan = dto.IdFoodPlan ;
            userDataEdit.IdGoal = dto.IdGoal ;
            userDataEdit.IdIntolerance = dto.IdIntolerance;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("UserData/{id}")]
        public async Task<IActionResult> GetUserDataById(int id )
        {
            UserData? userData = await _context.UserDatas.Where(ud=>ud.IdUser == id).FirstOrDefaultAsync();

            if (userData is null) { return NotFound("Ese usaurio no existe."); }

            UserDataDto userDataDto = new UserDataDto
            {
                IdUserData = userData.IdUserData,
                Gender = userData.Gender,
                Age = userData.Age,
                Height = userData.Height,
                Weight = userData.Weight,
                MealsPerDay = userData.MealsPerDay,
                IdUser = userData.IdUser,
                IdFoodPlan = userData.IdFoodPlan,
                IdGoal = userData.IdGoal,
                IdIntolerance = userData.IdIntolerance,
            };
            return Ok(userDataDto);
        }
    }
}
