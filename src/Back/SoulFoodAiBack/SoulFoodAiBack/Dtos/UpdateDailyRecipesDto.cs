namespace SoulFoodAiBack.Dtos
{
    public class UpdateDailyRecipesDto
    {
        public int IdUserFoodPlanDaily { get; set; }        
        public List<int> RecipeIds { get; set; } = new List<int>();
    }
}