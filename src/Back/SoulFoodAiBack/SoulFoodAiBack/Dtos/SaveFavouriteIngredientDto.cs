namespace SoulFoodAiBack.Dtos
{
    public class SaveFavouriteIngredientDto
    {
        public int UserId { get; set; }
        public int? LocalIngredientId { get; set; }
        public string? OpenFoodFactsId { get; set; }
        public string? Name { get; set; }
        public string? Brand { get; set; }
        public string? ImageUrl { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
        public double Kcal { get; set; }
    }
}
