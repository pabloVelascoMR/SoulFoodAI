using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Models
{
    public class UserIngredient
    {
        public UserIngredient()
        {
            this.CreationDate= DateTime.Now;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUserIngredient { get; set; }

        [Required]
        public required int IdUser { get; set; }

        [ForeignKey("IdUser")]
        [Required]
        public required User User { get; set; }

        [Required]
        public required int IdIngredient { get; set; }

        [ForeignKey("IdIngredient")]
        [Required]
        public required Ingredient Ingredient { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
