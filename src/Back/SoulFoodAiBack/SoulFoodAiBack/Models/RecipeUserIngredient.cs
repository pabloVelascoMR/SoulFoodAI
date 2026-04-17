using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Models
{
    public class RecipeUserIngredient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdRecipeUserIngredient { get; set; }

        [Required]
        public double Quantity { get; set; } 

        [Required]
        [StringLength(50)]
        public required string Unit { get; set; } 

        public int IdRecipe { get; set; }
        [ForeignKey("IdRecipe")]
        public Recipe? Recipe { get; set; }

        public int IdIngredient { get; set; }
        [ForeignKey("IdIngredient")]
        public Ingredient? Ingredient { get; set; }
    }
}