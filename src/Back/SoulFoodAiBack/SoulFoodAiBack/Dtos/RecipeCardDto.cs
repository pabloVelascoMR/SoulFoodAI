namespace SoulFoodAiBack.Dtos
{
    public class RecipeCardDto
    {
        public int IdRecipe { get; set; }
        public string RecipeName { get; set; } = string.Empty;
        public int Kcal { get; set; }
        public string MealName { get; set; } = string.Empty; 
        public string DietName { get; set; } = string.Empty; 
    }
}