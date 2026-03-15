using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Models
{
    public class UserData
    {
        public UserData()
        {
            this.CreationDate= DateTime.Now;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUserData { get; set; }

        [Required]
        public required string Gender { get; set; }

        public int Age { get; set; }

        public double Height { get; set; }

        public double Weight { get; set; }

        [Range(1, 5)]
        public int MealsPerDay { get; set; }

        [Required]
        public int IdUser { get; set; }

        [ForeignKey("IdUser")]
        public required User User { get; set; }

        public int IdFoodPlan { get; set; }

        [ForeignKey("IdFoodPlan")]
        public required FoodPlan FoodPlan { get; set; }

        public int IdGoal { get; set; }

        [ForeignKey("IdGoal")]
        public required Goal Goal { get; set; }

        public int IdIntolerance { get; set; }

        [ForeignKey("IdIntolerance")]
        public required Intolerance Intolerance { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
