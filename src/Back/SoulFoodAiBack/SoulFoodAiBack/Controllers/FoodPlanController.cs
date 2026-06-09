using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;

namespace SoulFoodAiBack.Controllers
{
    /// <summary>
    /// Controlador encargado de la gestión integral de los planes de alimentación (FoodPlans).
    /// Expone los endpoints necesarios para realizar las operaciones CRUD (Crear, Leer, Actualizar y Eliminar) sobre las dietas registradas.
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
    public class FoodPlanController : ControllerBase
    {
        /// <summary>
        /// Contexto de acceso a datos de Entity Framework Core utilizado para las transacciones con la base de datos.
        /// </summary>
        private readonly DataContext _context;

        /// <summary>
        /// Constructor de la clase que inyecta la dependencia del contexto de base de datos.
        /// </summary>
        /// <param name="dataContext">Instancia del contexto de Entity Framework.</param>
        public FoodPlanController(DataContext dataContext)
        {
            _context = dataContext;
        }

        /// <summary>
        /// Realiza una consulta a la base de datos para recuperar todos los planes de alimentación registrados.
        /// </summary>
        /// <returns>Devuelve un código 200 (OK) junto con una colección de objetos de transferencia (FoodPlanDto).</returns>
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

        /// <summary>
        /// Crea e inserta un nuevo plan de alimentación en la base de datos a partir de los datos recibidos.
        /// </summary>
        /// <param name="dto">Objeto de transferencia (CreateFoodPlanDto) que encapsula los datos requeridos para la creación.</param>
        /// <returns>Devuelve un código 200 (OK) tras confirmar la inserción exitosa.</returns>
        [HttpPost]
        [Route("AddFoodPlan")]
        public async Task<IActionResult> AddFoodPlan(CreateFoodPlanDto dto)
        {
            FoodPlan foodPlanAdd = new FoodPlan { FoodPlanName = dto.FoodPlanName, Description = dto.Description };
            await _context.FoodPlans.AddAsync(foodPlanAdd);
            await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Elimina físicamente un plan de alimentación del repositorio de datos en base a su identificador primario.
        /// </summary>
        /// <param name="idFoodPlan">Identificador único del plan de alimentación objetivo.</param>
        /// <returns>Retorna la lista actualizada de planes en caso de éxito, o un código 404 (Not Found) si el identificador no existe.</returns>
        [HttpDelete]
        [Route("DeleteFoodPlan")]
        public async Task<IActionResult> DeleteFoodPlan(int idFoodPlan)
        {
            FoodPlan? foodPlan = await _context.FoodPlans.FirstOrDefaultAsync(fp => fp.IdFoodPlan == idFoodPlan);

            if (foodPlan is null) { return NotFound("Ese plan no existe."); }

            _context.FoodPlans.Remove(foodPlan);
            await _context.SaveChangesAsync();
            return await GetAllFoodPlan();
        }

        /// <summary>
        /// Modifica las propiedades de un plan de alimentación existente mediante su identificador.
        /// </summary>
        /// <param name="dto">Objeto de transferencia (FoodPlanDto) que contiene el estado actualizado de la entidad.</param>
        /// <returns>Devuelve un código 200 (OK) si la persistencia es correcta, o un código 404 (Not Found) si no se halla el registro.</returns>
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