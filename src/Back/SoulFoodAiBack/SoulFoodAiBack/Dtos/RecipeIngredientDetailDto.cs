namespace SoulFoodAiBack.Dtos
{
    public class RecipeIngredientDetailDto
    {
        public string Name { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
}
