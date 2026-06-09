using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Models;
using SoulFoodAiBack.Services;

namespace SoulFoodAiBack.Controllers
{
    /// <summary>
    /// Controlador encargado de la gestión de identidades y perfiles de usuario, incluyendo los procesos de registro y eliminación de cuentas.
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
    public class UserController : ControllerBase
    {
        /// <summary>
        /// Contexto de base de datos inyectado para operaciones transaccionales sobre la entidad de usuarios.
        /// </summary>
        private readonly DataContext _context;

        /// <summary>
        /// Servicio de autenticación inyectado, utilizado para la generación de tokens JWT durante el flujo de registro.
        /// </summary>
        private readonly AuthService _authService;

        /// <summary>
        /// Constructor de la clase que provee la inyección de dependencias para el contexto de datos y el servicio de autenticación.
        /// </summary>
        /// <param name="dataContext">Instancia del contexto de Entity Framework.</param>
        /// <param name="authService">Instancia del servicio de autenticación y autorización.</param>
        public UserController(DataContext dataContext, AuthService authService)
        {
            _context = dataContext;
            _authService = authService;
        }

        /// <summary>
        /// Recupera la colección completa de todos los usuarios registrados en la plataforma (operación orientada a roles de administración).
        /// </summary>
        /// <returns>Devuelve un código 200 (OK) con la lista de usuarios.</returns>
        [HttpGet]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            List<User> users = await _context.Users.ToListAsync();

            List<UserDto> allUsers = users.
                Select(u => new UserDto
                {
                    IdUser = u.IdUser,
                    UserName = u.UserName,
                    Email = u.Email,
                    PasswordHash = u.PasswordHash

                }).ToList();

            return Ok(allUsers);
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema. Realiza la validación de unicidad del correo electrónico 
        /// y asegura la contraseña mediante un proceso de hashing criptográfico (BCrypt) previo a su almacenamiento.
        /// </summary>
        /// <param name="dto">Objeto de transferencia de datos con las credenciales e información de registro.</param>
        /// <returns>Devuelve un código 200 (OK) junto con el token JWT de sesión si el registro se efectúa correctamente.</returns>
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

        /// <summary>
        /// Ejecuta la eliminación permanente y en cascada de un usuario y de todos sus datos asociados en la plataforma.
        /// </summary>
        /// <param name="idUser">Identificador principal del usuario a eliminar.</param>
        /// <returns>Colección actualizada de usuarios tras efectuar la eliminación.</returns>
        [HttpDelete]
        [Route("DeleteUser")]
        public async Task<IActionResult> DeleteUser(int idUser)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.IdUser == idUser);

            if (user is null) { return NotFound("Ese usuario no existe."); }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return await GetAllUsers();
        }
    }
}