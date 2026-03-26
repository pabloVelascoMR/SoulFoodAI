using System.ComponentModel.DataAnnotations;

namespace SoulFoodAiBack.Dtos
{
    public class FoodPlanDto
    {
        public int IdFoodPlan { get; set; }
 
        public required string FoodPlanName { get; set; }

        public string? Description { get; set; }
    }
}
