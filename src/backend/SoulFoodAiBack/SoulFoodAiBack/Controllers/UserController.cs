using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using System.ComponentModel;

namespace SoulFoodAiBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly DataContext _context;

        public UserController(DataContext dataContext)
        {
            _context = dataContext;
        }

       [HttpGet]
       [Route("User")]

       public async Task<IActionResult> GetAllUsers()
        {
            List<User> usuarios = await _context.Users.ToListAsync();

            List<UserDto> allUsers =  usuarios.
                
                Select(u=> new UserDto { 
                    
                    IdUser=u.IdUser,
                    UserName=u.UserName,
                    Email =u.Email,
                    PasswordHash=u.PasswordHash

                }).ToList();    

            return Ok(allUsers);
           
        }

        [HttpPost]
        [Route("User")]

        public async Task<IActionResult> AddUser(CreateUserDto dto)
        {

            User userAdd = new User { UserName = dto.UserName, Email = dto.Email, PasswordHash = dto.PasswordHash };
            await _context.Users.AddAsync(userAdd);
            await _context.SaveChangesAsync();
            return Ok(userAdd);

        }
    }
}
