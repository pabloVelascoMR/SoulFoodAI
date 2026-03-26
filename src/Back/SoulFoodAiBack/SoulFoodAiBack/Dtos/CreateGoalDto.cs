namespace SoulFoodAiBack.Dtos
{
    public class CreateGoalDto
    {
        public required string GoalName { get; set; }
        public string? Description { get; set; }
    }
}
