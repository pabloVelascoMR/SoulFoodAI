using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using SoulFoodAiBack.Services;
using System.ComponentModel;

namespace SoulFoodAiBack.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {

        private readonly DataContext _context;
        private readonly AuthService _authService;

        public UserController(DataContext dataContext, AuthService authService)
        {
            _context = dataContext;
            _authService = authService;
        }

       [HttpGet]
       [Route("GetAllUsers")]

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
        [Route("AddUser")]
        public async Task<IActionResult> AddUser([FromBody] CreateUserDto dto)
        {
            
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Este correo ya existe.");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.PasswordHash);
            User userAdd = new User { UserName = dto.UserName, Email = dto.Email, PasswordHash = passwordHash };

            _context.Users.Add(userAdd);
            await _context.SaveChangesAsync();
            string token = _authService.GenerateJwtToken(userAdd);

            return Ok(new { IdUser = userAdd.IdUser, Token = token });
        }

        [HttpDelete]
        [Route("DeleteUser")]

        public async Task<IActionResult> DeleteUser(int idUser)
        {

            User? user = await _context.Users.FirstOrDefaultAsync(u=>u.IdUser== idUser);

            if (user is null) { return NotFound("Ese usuario no existe."); }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return await GetAllUsers(); ;

        }
    }
}
