using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Models
{
    public class UserFoodPlan
    {
        public UserFoodPlan()
        {
            UserFoodPlanMeals = new List<UserFoodPlanMeal>();
            this.CreationDate = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUserFoodPlan { get; set; }

        public int TotalDailyKcal { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public List<UserFoodPlanMeal> UserFoodPlanMeals { get; set; }

        public int IdUser { get; set; }

        [ForeignKey("IdUser")]
        [Required]
        public required User User { get; set; }

        public int IdFoodPlan { get; set; }

        [ForeignKey("IdFoodPlan")]
        [Required]
        public required  FoodPlan FoodPlan { get; set; }

        public DateTime CreationDate { get; set; }
    }

}
