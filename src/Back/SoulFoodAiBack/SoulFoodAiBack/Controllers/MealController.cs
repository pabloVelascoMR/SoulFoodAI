using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;

namespace SoulFoodAiBack.Controllers
{
    /// <summary>
    /// Controlador dedicado a la gestión de los momentos de ingesta o tipos de comida (ej. Desayuno, Almuerzo, Cena, Snack).
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
    public class MealController : ControllerBase
    {
        /// <summary>
        /// Contexto de acceso a datos de Entity Framework para las transacciones persistentes.
        /// </summary>
        private readonly DataContext _context;

        /// <summary>
        /// Constructor de la clase que provee la inyección de dependencias para el contexto de datos.
        /// </summary>
        /// <param name="dataContext">Referencia inyectada al contexto de base de datos.</param>
        public MealController(DataContext dataContext)
        {
            _context = dataContext;
        }

        /// <summary>
        /// Consulta y devuelve todos los momentos de ingesta catalogados en el sistema de manera global.
        /// </summary>
        /// <returns>Devuelve un código 200 (OK) con la colección de los diferentes tipos de comida disponibles.</returns>
        [HttpGet]
        [Route("GetAllMeals")]
        public async Task<IActionResult> GetAllMeals()
        {
            List<Meal> meals = await _context.Meals.ToListAsync();

            List<MealDto> allmeals = meals.

                Select(m => new MealDto
                {
                    IdMeal = m.IdMeal,
                    MealName = m.MealName,

                }).ToList();

            return Ok(allmeals);
        }

        /// <summary>
        /// Registra un nuevo tipo de comida o momento de ingesta en el catálogo global del sistema.
        /// </summary>
        /// <param name="dto">Objeto de transferencia que incluye la denominación de la ingesta.</param>
        /// <returns>Devuelve un código 200 (OK) si la persistencia se completa de forma satisfactoria.</returns>
        [HttpPost]
        [Route("AddMeal")]
        public async Task<IActionResult> AddMeal(CreateMealDto dto)
        {
            Meal mealAdd = new Meal { MealName = dto.MealName };
            await _context.Meals.AddAsync(mealAdd);
            await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Elimina de forma permanente un registro de tipo de comida del sistema mediante su identificador primario.
        /// </summary>
        /// <param name="idMeal">Identificador unívoco del tipo de comida que se persigue eliminar.</param>
        /// <returns>La colección persistente tras el borrado, o código 404 (Not Found) si el elemento no consta.</returns>
        [HttpDelete]
        [Route("DeleteMeal")]
        public async Task<IActionResult> DeleteMeal(int idMeal)
        {
            Meal? meal = await _context.Meals.FirstOrDefaultAsync(m => m.IdMeal == idMeal);

            if (meal is null) { return NotFound("Esa comida no existe."); }

            _context.Meals.Remove(meal);
            await _context.SaveChangesAsync();
            return await GetAllMeals();
        }

        /// <summary>
        /// Modifica las características de un tipo de comida registrado en la base de datos, actualizando su nomenclatura.
        /// </summary>
        /// <param name="dto">Objeto de transferencia de datos con el identificador y el estado modificado.</param>
        /// <returns>Devuelve un código 200 (OK) en caso de actualización exitosa.</returns>
        [HttpPut]
        [Route("EditMeal")]
        public async Task<IActionResult> EditMeal(MealDto dto)
        {
            Meal? mealEdit = await _context.Meals.FirstOrDefaultAsync(m => m.IdMeal == dto.IdMeal);

            if (mealEdit is null) { return NotFound("Comida no existe en la base de datos."); }

            mealEdit.IdMeal = dto.IdMeal;
            mealEdit.MealName = dto.MealName;

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}