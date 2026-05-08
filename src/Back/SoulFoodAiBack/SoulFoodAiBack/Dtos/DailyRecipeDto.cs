namespace SoulFoodAiBack.Dtos
{
   
    public class DailyRecipeDto
    {
        public int IdRecipe { get; set; }
        public string RecipeName { get; set; } = string.Empty;
        public int Kcal { get; set; }
        public string MealType { get; set; } = string.Empty; 
    }
}