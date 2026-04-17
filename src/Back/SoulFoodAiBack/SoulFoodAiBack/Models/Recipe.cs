using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Models
{
    public class Recipe
    {
        public Recipe()
        {
            FoodPlanDailyRecipes = new List<FoodPlanDailyRecipe>();
            RecipeUserIngredients = new List<RecipeUserIngredient>();
            this.CreationDate = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdRecipe { get; set; }

        [Required]
        [StringLength(500)]
        public required string RecipeName { get; set; }

        public double Protein { get; set; }

        public double Carbs { get; set; }

        public double Fat { get; set; }

        public int TotalKcal { get; set; }

        public String? RecipeDescription { get; set; }

        public int IdUser { get; set; }
        [ForeignKey("IdUser")]
        public User? User { get; set; }

        public int IdMeal { get; set; }
        [ForeignKey("IdMeal")]
        public Meal? Meal { get; set; }
        
        public List<FoodPlanDailyRecipe> FoodPlanDailyRecipes { get; set; }
        public List<RecipeUserIngredient> RecipeUserIngredients { get; set; }

        public DateTime CreationDate { get; set; }
    }
}

