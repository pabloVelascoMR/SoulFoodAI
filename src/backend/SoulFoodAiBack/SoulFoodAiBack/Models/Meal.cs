using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Models
{
    public class Meal
    {
        public Meal()
        {
            this.CreationDate = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdMeal { get; set; }

        [Required]
        [StringLength(100)]
        public required string MealName { get; set; }

        public DateTime CreationDate { get; set; }
    }
}

