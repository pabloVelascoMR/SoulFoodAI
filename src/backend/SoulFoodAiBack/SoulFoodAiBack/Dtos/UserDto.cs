namespace SoulFoodAiBack.Dtos
{
    public class UserDto
    {
        public int IdUser { get; set; }

        public required string UserName { get; set; }

        public required string Email { get; set; }

        public required string PasswordHash { get; set; }
    }
}
