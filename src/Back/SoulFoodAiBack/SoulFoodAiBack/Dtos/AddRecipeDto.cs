using SoulFoodAiBack.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Dtos
{
    public class AddRecipeDto
    {
        public required string RecipeName { get; set; }

        public int IdMeal { get; set; }

       public List<int> IdIngredients { get; set; }

       public List<float> Quantity { get; set; }

       public List<string> Unit { get; set; }

        public String? RecipeDescription { get; set; }

    }
}
