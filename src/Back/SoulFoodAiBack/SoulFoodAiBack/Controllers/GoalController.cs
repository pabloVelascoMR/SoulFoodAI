using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;

namespace SoulFoodAiBack.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar los objetivos nutricionales y físicos de los usuarios.
    /// Expone los endpoints necesarios para la manipulación y consulta del catálogo de objetivos del sistema.
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
    public class GoalController : Controller
    {
        /// <summary>
        /// Contexto de datos inyectado para la interacción con la base de datos subyacente mediante Entity Framework.
        /// </summary>
        private readonly DataContext _context;

        /// <summary>
        /// Constructor de la clase que inyecta la dependencia del contexto de base de datos.
        /// </summary>
        /// <param name="dataContext">Instancia del contexto de Entity Framework.</param>
        public GoalController(DataContext dataContext)
        {
            _context = dataContext;
        }

        /// <summary>
        /// Consulta la base de datos para recuperar la totalidad de los objetivos registrados en el sistema.
        /// </summary>
        /// <returns>Devuelve un código 200 (OK) con la lista de objetos GoalDto.</returns>
        [HttpGet]
        [Route("GetAllGoals")]
        public async Task<IActionResult> GetAllGoals()
        {
            List<Goal> goals = await _context.Goals.ToListAsync();

            List<GoalDto> allgoals = goals.
                Select(g => new GoalDto
                {
                    IdGoal = g.IdGoal,
                    GoalName = g.GoalName,
                    Description = g.Description
                }).ToList();

            return Ok(allgoals);
        }

        /// <summary>
        /// Inserta un nuevo registro de objetivo en el sistema a partir de los datos proporcionados.
        /// </summary>
        /// <param name="dto">Objeto de transferencia de datos con la información del nuevo objetivo.</param>
        /// <returns>Devuelve un código 200 (OK) tras confirmar la inserción exitosa.</returns>
        [HttpPost]
        [Route("AddGoal")]
        public async Task<IActionResult> AddGoal(CreateGoalDto dto)
        {
            Goal goalAdd = new Goal { GoalName = dto.GoalName, Description = dto.Description };
            await _context.Goals.AddAsync(goalAdd);
            await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Realiza la eliminación física de un objetivo en el repositorio de datos mediante su identificador único.
        /// </summary>
        /// <param name="idGoal">Identificador primario del objetivo a eliminar.</param>
        /// <returns>La colección actualizada de objetivos, o código 404 (Not Found) si no se halla el registro.</returns>
        [HttpDelete]
        [Route("DeleteGoal")]
        public async Task<IActionResult> DeleteGoal(int idGoal)
        {
            Goal? goal = await _context.Goals.FirstOrDefaultAsync(g => g.IdGoal == idGoal);

            if (goal is null) { return NotFound("Ese objetivo no existe."); }

            _context.Goals.Remove(goal);
            await _context.SaveChangesAsync();
            return await GetAllGoals();
        }

        /// <summary>
        /// Modifica las propiedades de un objetivo existente basándose en la información proporcionada en el objeto de transferencia.
        /// </summary>
        /// <param name="dto">Objeto de transferencia de datos con las modificaciones a aplicar.</param>
        /// <returns>Devuelve un código 200 (OK) en caso de éxito, o código 404 (Not Found) si no existe el identificador.</returns>
        [HttpPut]
        [Route("EditGoal")]
        public async Task<IActionResult> EditGoal(GoalDto dto)
        {
            Goal? goalEdit = await _context.Goals.FirstOrDefaultAsync(g => g.IdGoal == dto.IdGoal);

            if (goalEdit is null) { return NotFound("Objetivo no existe en la base de datos."); }

            goalEdit.IdGoal = dto.IdGoal;
            goalEdit.GoalName = dto.GoalName;
            goalEdit.Description = dto.Description;

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}