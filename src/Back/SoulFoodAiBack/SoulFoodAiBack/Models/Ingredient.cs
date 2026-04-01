using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Models
{
    public class Ingredient
    {
        public Ingredient()
        {
            this.CreationDate = DateTime.Now;
            this.UserIngredients = new List<UserIngredient>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdIngredient { get; set; }

        [Required]
        [MaxLength(150)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Category { get; set; }

        [MaxLength(50)]
        public string? SubCategory { get; set; }

        [MaxLength(100)]
        public string? Brand { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public int? CreatedByUserId { get; set; }

        [MaxLength(50)]
        public string? Icon { get; set; }

        public double Protein { get; set; }

        public double Carbs { get; set; }

        public double Fat { get; set; }

        public double Kcal { get; set; }

        [MaxLength(100)]
        public string? OpenFoodFactsId { get; set; }

        public bool IsDeleted { get; set; } = false;

        public List<UserIngredient> UserIngredients { get; set; } 

        public DateTime CreationDate { get; set; }
    }
}
