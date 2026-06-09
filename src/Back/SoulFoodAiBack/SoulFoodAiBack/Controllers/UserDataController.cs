using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;

namespace SoulFoodAiBack.Controllers
{
    /// <summary>
    /// Controlador responsable de gestionar los perfiles biométricos y la configuración nutricional de los usuarios, 
    /// incluyendo su antropometría, metas y restricciones dietéticas.
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
    public class UserDataController : ControllerBase
    {
        /// <summary>
        /// Contexto de acceso a datos para interactuar de forma transaccional con la base de datos.
        /// </summary>
        private readonly DataContext _context;

        /// <summary>
        /// Constructor de la clase que inyecta la dependencia del contexto de Entity Framework.
        /// </summary>
        /// <param name="dataContext">Instancia inyectada del contexto de datos.</param>
        public UserDataController(DataContext dataContext)
        {
            _context = dataContext;
        }

        /// <summary>
        /// Consulta y devuelve la totalidad de los perfiles biométricos almacenados en el sistema.
        /// </summary>
        /// <returns>Devuelve un código 200 (OK) con la colección completa de perfiles.</returns>
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
                    IdUserData = ud.IdUserData,
                    Gender = ud.Gender,
                    Age = ud.Age,
                    Height = ud.Height,
                    Weight = ud.Weight,
                    MealsPerDay = ud.MealsPerDay,
                    IdUser = ud.IdUser,
                    IdFoodPlan = ud.IdFoodPlan,
                    IdGoal = ud.IdGoal,
                    IdIntolerances = ud.UserIntolerances.Select(ui => ui.IdIntolerance).ToList()
                }).ToList();

            return Ok(allUserDatas);
        }

        /// <summary>
        /// Inicializa el perfil biométrico de un usuario de nueva creación, asignando valores por defecto 
        /// a las métricas que no hayan sido proporcionadas explícitamente en la solicitud inicial.
        /// </summary>
        /// <param name="dto">Objeto de transferencia con los datos biométricos fundamentales aportados.</param>
        /// <returns>Devuelve un código 200 (OK) tras confirmar la inicialización del perfil.</returns>
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

            if (dto.IdIntolerances != null && dto.IdIntolerances.Count > 0)
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

        /// <summary>
        /// Elimina físicamente el perfil de datos biométricos asociado a un usuario específico.
        /// </summary>
        /// <param name="idUser">Identificador principal del usuario propietario del perfil.</param>
        /// <returns>La colección actualizada de perfiles tras confirmar la operación.</returns>
        [HttpDelete]
        [Route("DeleteUserData")]
        public async Task<IActionResult> DeleteUserData(int idUser)
        {
            UserData? userData = await _context.UserDatas.FirstOrDefaultAsync(u => u.IdUser == idUser);

            if (userData is null) { return NotFound("Ese usaurio no existe."); }

            _context.UserDatas.Remove(userData);
            await _context.SaveChangesAsync();
            return await GetAllUserDatas();
        }

        /// <summary>
        /// Actualiza integralmente el perfil biométrico del usuario, gestionando de forma sincronizada 
        /// el reemplazo total de sus relaciones con intolerancias dietéticas.
        /// </summary>
        /// <param name="dto">Objeto de transferencia con el nuevo estado del perfil.</param>
        /// <returns>Devuelve un código 200 (OK) si la actualización transaccional se completa con éxito.</returns>
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
            userDataEdit.IdFoodPlan = dto.IdFoodPlan;
            userDataEdit.IdGoal = dto.IdGoal;

            _context.UserIntolerances.RemoveRange(userDataEdit.UserIntolerances);

            if (dto.IdIntolerances != null && dto.IdIntolerances.Count > 0)
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

        /// <summary>
        /// Recupera de forma individual el perfil biométrico de un usuario basándose en su identificador, 
        /// proveyendo una estructura predeterminada (vacía) en caso de que aún no exista un registro real.
        /// </summary>
        /// <param name="id">Identificador unívoco del usuario.</param>
        /// <returns>Objeto de transferencia de datos con la configuración del perfil.</returns>
        [HttpGet]
        [Route("GetUserDataById/{id}")]
        public async Task<IActionResult> GetUserDataById(int id)
        {
            UserData? userData = await _context.UserDatas
                 .Include(ud => ud.UserIntolerances)
                 .AsNoTracking()
                 .FirstOrDefaultAsync(ud => ud.IdUser == id);

            if (userData is null) { return Ok(new { IdFoodPlan = 1 }); }

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

        /// <summary>
        /// Aplica una actualización parcial sobre el perfil del usuario, modificando exclusivamente sus medidas corporales (antropometría) sin alterar sus métricas base.
        /// </summary>
        /// <param name="dto">Objeto de transferencia que contiene únicamente las medidas corporales a actualizar.</param>
        /// <returns>Devuelve un código 200 (OK) tras actualizar el registro.</returns>
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