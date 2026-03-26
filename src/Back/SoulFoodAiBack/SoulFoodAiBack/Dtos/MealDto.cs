using System.ComponentModel.DataAnnotations;

namespace SoulFoodAiBack.Dtos
{
    public class MealDto
    {
       public int IdMeal { get; set; }
       public required string MealName { get; set; }
    }
}
