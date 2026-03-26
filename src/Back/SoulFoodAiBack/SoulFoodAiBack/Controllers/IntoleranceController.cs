using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;

namespace SoulFoodAiBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IntoleranceController: ControllerBase
    {
        private readonly DataContext _context;

        public IntoleranceController(DataContext dataContext)
        {
            _context = dataContext;
        }

        [HttpGet]
        [Route("Intolerance")]

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

        [HttpPost]
        [Route("Intolerance")]

        public async Task<IActionResult> AddGoal(CreateIntoleranceDto dto)
        {

            Intolerance intoleranceAdd = new Intolerance { IntoleranceName = dto.IntoleranceName};
            await _context.Intolerances.AddAsync(intoleranceAdd);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        [Route("Intolerance")]

        public async Task<IActionResult> DeleteIntolerance(int idIntolerance)
        {

            Intolerance? intolerance = await _context.Intolerances.FirstOrDefaultAsync(i => i.IdIntolerance == idIntolerance);

            if (intolerance is null) { return NotFound("Ese objetivo no existe."); }

            _context.Intolerances.Remove(intolerance);
            await _context.SaveChangesAsync();
            return await GetAllIntolerances(); ;
        }

        [HttpPut]
        [Route("Intolerance")]

        public async Task<IActionResult> EditIntolerance(IntoleranceDto dto)
        {
            Intolerance? intoleranceEdit = await _context.Intolerances.FirstOrDefaultAsync(i => i.IdIntolerance == dto.IdIntolerance);

            if (intoleranceEdit is null) { return NotFound("Competicion no existe en la base de datos."); }

            intoleranceEdit.IdIntolerance = dto.IdIntolerance;
            intoleranceEdit.IntoleranceName = dto.IntoleranceName;
      
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
