namespace SoulFoodAiBack.Dtos
{
    public class CreateFoodPlanDto
    {
        public required string FoodPlanName { get; set; }

        public string? Description { get; set; }
    }
}
