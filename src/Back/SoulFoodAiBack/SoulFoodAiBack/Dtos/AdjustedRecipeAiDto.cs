namespace SoulFoodAiBack.Dtos
{
    public class AdjustedRecipeAiDto
    {
        public int idFoodPlanDailyRecipe { get; set; }
        public List<AdjustedIngredientAiDto> ingredients { get; set; }
    }
}
