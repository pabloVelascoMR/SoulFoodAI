using System.ComponentModel.DataAnnotations;

namespace SoulFoodAiBack.Dtos
{
    public class GoalDto
    {
        public int IdGoal { get; set; }
        public required string GoalName { get; set; }
    }
}
