using System.ComponentModel.DataAnnotations;

namespace SoulFoodAiBack.Dtos
{
    public class CreateMealDto
    {
        public required string MealName { get; set; }
    }
}
