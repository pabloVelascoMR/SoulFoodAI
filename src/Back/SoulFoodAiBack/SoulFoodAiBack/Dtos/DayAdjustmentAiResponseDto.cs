namespace SoulFoodAiBack.Dtos
{
    public class DayAdjustmentAiResponseDto
    {
        public bool isPossible { get; set; }
        public string errorMessage { get; set; }
        public List<AdjustedRecipeAiDto> adjustedRecipes { get; set; }
    }
}
