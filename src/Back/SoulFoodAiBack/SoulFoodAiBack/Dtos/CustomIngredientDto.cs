namespace SoulFoodAiBack.Dtos
{
    public class CustomIngredientDto
    {
        public required string Name { get; set; }
        public string? Brand { get; set; }
        public string? Icon { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
        public double Kcal { get; set; }
        public required string UserId { get; set; }
    }
}
