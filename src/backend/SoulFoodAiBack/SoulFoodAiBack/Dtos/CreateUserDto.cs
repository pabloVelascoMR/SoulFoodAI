using SoulFoodAiBack.Models;
using System.ComponentModel.DataAnnotations;

namespace SoulFoodAiBack.Dtos
{
    public class CreateUserDto
    {
        public required string UserName { get; set; }

        public required string Email { get; set; }

        public required string PasswordHash { get; set; }

    }
}
