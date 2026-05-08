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
        [Route("GetAllUserDatas")]

        public async Task<IActionResult> GetAllUserDatas()
        {
        
            List<UserData> userDatas = await _context.UserDatas
                .Include(ud => ud.UserIntolerances)
                .ToListAsync();

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
                    IdIntolerances = ud.UserIntolerances.Select(ui => ui.IdIntolerance).ToList()
                }).ToList();

            return Ok(allUserDatas);
        }

        [HttpPost]
        [Route("AddUserData")]
        public async Task<IActionResult> AddUserData(CreateUserDataDto dto)
        {
            

            UserData userDataAdd = new UserData 
            {
                Gender = dto.Gender,
                Age = dto.Age,
                Height = dto.Height,
                Weight = dto.Weight,
                MealsPerDay = dto.MealsPerDay > 0 ? dto.MealsPerDay : 3,
                LevelOfActivity = dto.LevelOfActivity > 0 ? dto.LevelOfActivity : 1,
                ChestMeasure = dto.ChestMeasure ?? 0,
                WaistMeasure = dto.WaistMeasure ?? 0,
                HipMeasure = dto.HipMeasure ?? 0,
                LeftBicepMeasure = dto.LeftBicepMeasure ?? 0,
                RightBicepMeasure = dto.RightBicepMeasure ?? 0,
                LeftCuadricepsMeasure = dto.LeftCuadricepsMeasure ?? 0,
                RightCuadricepsMeasure = dto.RightCuadricepsMeasure ?? 0,

                IdUser = dto.IdUser,
                IdFoodPlan = dto.IdFoodPlan > 0 ? dto.IdFoodPlan : 2,
                IdGoal = dto.IdGoal > 0 ? dto.IdGoal : 6,
                UserIntolerances = new List<UserIntolerance>()
            };

            if (dto.IdIntolerances != null && dto.IdIntolerances.Any())
            {
                foreach (var idIntolerance in dto.IdIntolerances)
                {
                    userDataAdd.UserIntolerances.Add(new UserIntolerance
                    {
                        IdIntolerance = idIntolerance,
                        IdUser = dto.IdUser 
                    });
                }
            }
            await _context.UserDatas.AddAsync(userDataAdd);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        [Route("DeleteUserData")]
        public async Task<IActionResult> DeleteUserData(int idUser)
        {

            UserData? userData = await _context.UserDatas.FirstOrDefaultAsync(u => u.IdUser == idUser);

            if (userData is null) { return NotFound("Ese usaurio no existe."); }

            _context.UserDatas.Remove(userData);
            await _context.SaveChangesAsync();
            return await GetAllUserDatas(); ;
        }

        [HttpPut]
        [Route("EditUserData")]

        public async Task<IActionResult> EditUserData(UserDataDto dto)
        {
            UserData? userDataEdit = await _context.UserDatas
                .Include(u => u.UserIntolerances)
                .FirstOrDefaultAsync(u => u.IdUser == dto.IdUser);

            if (userDataEdit is null) { return NotFound("Objetivo no existe en la base de datos."); }

            userDataEdit.IdGoal = dto.IdGoal;
            userDataEdit.Gender = dto.Gender;
            userDataEdit.Age = dto.Age;
            userDataEdit.Height = dto.Height;
            userDataEdit.Weight = dto.Weight;
            userDataEdit.MealsPerDay = dto.MealsPerDay;
            userDataEdit.LevelOfActivity = dto.LevelOfActivity;
            userDataEdit.ChestMeasure = dto.ChestMeasure ?? 0;
            userDataEdit.WaistMeasure = dto.WaistMeasure ?? 0;
            userDataEdit.HipMeasure = dto.HipMeasure ?? 0;
            userDataEdit.LeftBicepMeasure = dto.LeftBicepMeasure ?? 0;
            userDataEdit.RightBicepMeasure = dto.RightBicepMeasure ?? 0;
            userDataEdit.LeftCuadricepsMeasure = dto.LeftCuadricepsMeasure ?? 0;
            userDataEdit.RightCuadricepsMeasure = dto.RightCuadricepsMeasure ?? 0;
            userDataEdit.IdUser = dto.IdUser;
            userDataEdit.IdFoodPlan = dto.IdFoodPlan ;
            userDataEdit.IdGoal = dto.IdGoal ;
            _context.UserIntolerances.RemoveRange(userDataEdit.UserIntolerances);

            if(dto.IdIntolerances != null && dto.IdIntolerances.Any())
            {
                userDataEdit.UserIntolerances = dto.IdIntolerances.Select(id => new UserIntolerance
                {
                    IdUser = dto.IdUser,
                    IdIntolerance = id
                }).ToList();
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        [Route("GetUserDataById/{id}")]
        public async Task<IActionResult> GetUserDataById(int id)
        {
            UserData? userData = await _context.UserDatas
                 .Include(ud => ud.UserIntolerances)
                 .AsNoTracking()
                 .FirstOrDefaultAsync(ud => ud.IdUser == id);

            if (userData is null) {return Ok(new { IdFoodPlan = 1 });}

            UserDataDto userDataDto = new UserDataDto
            {
                IdUserData = userData.IdUserData,
                Gender = userData.Gender,
                Age = userData.Age,
                Height = userData.Height,
                Weight = userData.Weight,
                MealsPerDay = userData.MealsPerDay,
                LevelOfActivity = userData.LevelOfActivity,
                ChestMeasure = userData.ChestMeasure,
                WaistMeasure = userData.WaistMeasure,
                HipMeasure = userData.HipMeasure,
                LeftBicepMeasure = userData.LeftBicepMeasure,
                RightBicepMeasure = userData.RightBicepMeasure,
                LeftCuadricepsMeasure = userData.LeftCuadricepsMeasure,
                RightCuadricepsMeasure = userData.RightCuadricepsMeasure,
                IdUser = userData.IdUser,
                IdFoodPlan = userData.IdFoodPlan,
                IdGoal = userData.IdGoal,
                IdIntolerances = userData.UserIntolerances.Select(ui => ui.IdIntolerance).ToList()
            };

            return Ok(userDataDto);
        }

        [HttpPut]
        [Route("UpdateBodyMeasures")]
        public async Task<IActionResult> UpdateBodyMeasures([FromBody] UpdateBodyMeasuresDto dto)
        {
            UserData? userData = await _context.UserDatas.FirstOrDefaultAsync(u => u.IdUser == dto.IdUser);

            if (userData == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            userData.ChestMeasure = dto.ChestMeasure;
            userData.WaistMeasure = dto.WaistMeasure;
            userData.HipMeasure = dto.HipMeasure;
            userData.LeftBicepMeasure = dto.LeftBicepMeasure;
            userData.RightBicepMeasure = dto.RightBicepMeasure;
            userData.LeftCuadricepsMeasure = dto.LeftCuadricepsMeasure;
            userData.RightCuadricepsMeasure = dto.RightCuadricepsMeasure;

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
