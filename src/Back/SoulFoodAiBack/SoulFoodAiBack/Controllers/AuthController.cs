using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoulFoodAiBack.Data;
using SoulFoodAiBack.Dtos;
using SoulFoodAiBack.Services;

namespace SoulFoodAiBack.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar el proceso de autenticación y autorización de los usuarios.
    /// Actúa como punto de entrada para verificar la identidad del usuario frente a la base de datos
    /// y emitir los tokens JWT necesarios para proteger las rutas de la API.
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
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// Contexto de acceso a datos proporcionado por Entity Framework Core para interactuar con la base de datos.
        /// </summary>
        private readonly DataContext _context;

        /// <summary>
        /// Servicio inyectado que centraliza la lógica de negocio relativa a la autenticación, como la emisión de tokens JWT.
        /// </summary>
        private readonly AuthService _authService;

        /// <summary>
        /// Constructor de la clase que inyecta las dependencias necesarias mediante el contenedor de inyección de dependencias (DI).
        /// </summary>
        /// <param name="context">Instancia del contexto de la base de datos.</param>
        /// <param name="authService">Instancia del servicio de autenticación.</param>
        public AuthController(DataContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        /// <summary>
        /// Procesa la solicitud de inicio de sesión de un usuario.
        /// Consulta la base de datos mediante el correo electrónico proporcionado y verifica de forma segura 
        /// que la contraseña introducida coincida con el hash almacenado mediante el algoritmo BCrypt.
        /// </summary>
        /// <param name="dto">Objeto de transferencia de datos (DTO) que contiene las credenciales del usuario.</param>
        /// <returns>Devuelve un código 200 (OK) con el token JWT si la validación es exitosa, o un código 401 (Unauthorized) en caso contrario.</returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // Busca al primer usuario que coincida con el email proporcionado
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            // Comprueba si el usuario no existe o si la contrase a introducida no coincide con el hash guardado
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Credenciales incorrectas.");

            // Si todo es correcto, genera el token JWT para la sesi
            string token = _authService.GenerateJwtToken(user);
            return Ok(new { IdUser = user.IdUser, Token = token });
        }
    }
}