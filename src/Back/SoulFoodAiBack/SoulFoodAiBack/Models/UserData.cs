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
        [Required]
        public User User { get; set; }

        public int IdFoodPlan { get; set; }

        [ForeignKey("IdFoodPlan")]
        [Required]
        public FoodPlan FoodPlan { get; set; }

        public int IdGoal { get; set; }

        [ForeignKey("IdGoal")]
        [Required]
        public  Goal Goal { get; set; }

        public int IdIntolerance { get; set; }

        public List<UserIntolerance> UserIntolerances { get; set; } = new List<UserIntolerance>();

        public DateTime CreationDate { get; set; }
    }
}
