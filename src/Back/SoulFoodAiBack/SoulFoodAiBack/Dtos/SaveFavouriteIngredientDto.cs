namespace SoulFoodAiBack.Dtos
{
    public class SaveFavouriteIngredientDto
    {
        public int IdUser { get; set; }
        public int? LocalIdIngredient { get; set; }
        public string? IdOpenFoodFacts { get; set; }
        public string? Name { get; set; }
        public string? Brand { get; set; }
        public string? Category { get; set; }
        public string? ImageUrl { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
        public double Kcal { get; set; }
    }
}
