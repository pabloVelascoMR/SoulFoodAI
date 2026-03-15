using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Models
{
    public class Recipe
    {
        public Recipe()
        {
            UserFoodPlanMeals = new List<UserFoodPlanMeal>();
            this.CreationDate = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdRecipe { get; set; }

        [Required]
        [StringLength(500)]
        public required string RecipeName { get; set; }

        [Required]
        public required string IngredientsJson { get; set; }

        public double Protein { get; set; }

        public double Carbs { get; set; }

        public double Fat { get; set; }

        public int TotalKcal { get; set; }

        public List<UserFoodPlanMeal> UserFoodPlanMeals { get; set; }

        public DateTime CreationDate { get; set; }
    }
}

