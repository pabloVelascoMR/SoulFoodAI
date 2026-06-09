using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;

namespace SoulFoodAiBack.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar el catálogo maestro de intolerancias, alergias y restricciones dietéticas del sistema.
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
    public class IntoleranceController : ControllerBase
    {
        /// <summary>
        /// Contexto de acceso a datos para realizar operaciones de persistencia sobre la base de datos relacional.
        /// </summary>
        private readonly DataContext _context;

        /// <summary>
        /// Constructor de la clase que inyecta la dependencia del contexto de datos de Entity Framework.
        /// </summary>
        /// <param name="dataContext">Instancia del contexto de acceso a datos.</param>
        public IntoleranceController(DataContext dataContext)
        {
            _context = dataContext;
        }

        /// <summary>
        /// Recupera de la base de datos todas las intolerancias y restricciones registradas en el sistema.
        /// </summary>
        /// <returns>Devuelve un código 200 (OK) con la colección completa de intolerancias.</returns>
        [HttpGet]
        [Route("GetAllIntolerances")]
        public async Task<IActionResult> GetAllIntolerances()
        {
            List<Intolerance> intolerances = await _context.Intolerances.ToListAsync();

            List<IntoleranceDto> allintolerances = intolerances.

                Select(i => new IntoleranceDto
                {
                    IdIntolerance = i.IdIntolerance,
                    IntoleranceName = i.IntoleranceName

                }).ToList();

            return Ok(allintolerances);
        }

        /// <summary>
        /// Inserta un nuevo registro de intolerancia o alergia en el catálogo global.
        /// </summary>
        /// <param name="dto">Objeto de transferencia que contiene la nomenclatura de la nueva intolerancia.</param>
        /// <returns>Devuelve un código 200 (OK) en caso de éxito en la inserción.</returns>
        [HttpPost]
        [Route("AddIntolerance")]
        public async Task<IActionResult> AddIntolerance(CreateIntoleranceDto dto)
        {
            Intolerance intoleranceAdd = new Intolerance { IntoleranceName = dto.IntoleranceName };
            await _context.Intolerances.AddAsync(intoleranceAdd);
            await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Elimina físicamente una intolerancia del catálogo maestro mediante su identificador primario.
        /// </summary>
        /// <param name="idIntolerance">Identificador numérico de la intolerancia.</param>
        /// <returns>Retorna la colección actualizada, o un código 404 (Not Found) si no existe dicho identificador.</returns>
        [HttpDelete]
        [Route("DeleteIntolerance")]
        public async Task<IActionResult> DeleteIntolerance(int idIntolerance)
        {
            Intolerance? intolerance = await _context.Intolerances.FirstOrDefaultAsync(i => i.IdIntolerance == idIntolerance);

            if (intolerance is null) { return NotFound("Esa intolerancia no existe."); }

            _context.Intolerances.Remove(intolerance);
            await _context.SaveChangesAsync();
            return await GetAllIntolerances();
        }

        /// <summary>
        /// Actualiza las propiedades de una intolerancia registrada previamente en la base de datos.
        /// </summary>
        /// <param name="dto">Objeto de transferencia de datos con el estado modificado de la entidad.</param>
        /// <returns>Devuelve un código 200 (OK) tras confirmar la edición exitosa.</returns>
        [HttpPut]
        [Route("EditIntolerance")]
        public async Task<IActionResult> EditIntolerance(IntoleranceDto dto)
        {
            Intolerance? intoleranceEdit = await _context.Intolerances.FirstOrDefaultAsync(i => i.IdIntolerance == dto.IdIntolerance);

            if (intoleranceEdit is null) { return NotFound("Intolerancia no existe en la base de datos."); }

            intoleranceEdit.IdIntolerance = dto.IdIntolerance;
            intoleranceEdit.IntoleranceName = dto.IntoleranceName;

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}