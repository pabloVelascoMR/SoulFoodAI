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
        public string Name { get; set; }

        public string Category { get; set; }

        public string SubCategory { get; set; }

        public double Protein { get; set; }

        public double Carbs { get; set; }

        public double Fat { get; set; }

        public double Kcal { get; set; }

        [Required]
        public int IdUser { get; set; }

        [ForeignKey("IdUser")]
        public User User { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
