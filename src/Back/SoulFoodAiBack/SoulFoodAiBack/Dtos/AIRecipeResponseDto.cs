namespace SoulFoodAiBack.Dtos
{
    public class AIRecipeResponseDto
    {
        public string RecipeName { get; set; }
        public string RecipeDescription { get; set; }

        public List<AIIngredientDto> Ingredients { get; set; }
    }
}
