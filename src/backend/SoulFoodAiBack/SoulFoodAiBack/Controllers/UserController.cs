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
            List<User> users = await _context.Users.ToListAsync();

            List<UserDto> allUsers =  users.
                
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
            string pass = dto.PasswordHash;
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(pass);

            User userAdd = new User { UserName = dto.UserName, Email = dto.Email, PasswordHash = passwordHash };
            await _context.Users.AddAsync(userAdd);
            await _context.SaveChangesAsync();
            return Ok(userAdd);

        }

        [HttpDelete]
        [Route("User")]

        public async Task<IActionResult> DeleteUser(int idUser)
        {

            User? user = await _context.Users.FirstOrDefaultAsync(u=>u.IdUser== idUser);

            if (user is null) { return NotFound("Ese equipo no existe en la competición."); }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return await GetAllUsers(); ;

        }
    }
}
