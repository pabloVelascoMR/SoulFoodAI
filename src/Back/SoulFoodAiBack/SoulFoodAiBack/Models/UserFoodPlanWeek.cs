using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Models
{
    public class UserFoodPlanWeek
    {
        public UserFoodPlanWeek()
        {
            UserFoodPlanMeals = new List<UserFoodPlanDaily>();
            this.CreationDate = DateTime.Now;
            IsActive = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUserFoodPlanWeek { get; set; }

        public int TotalWeeklyKcal { get; set; }

        public double TotalWeeklyProtein { get; set; }
        public double TotalWeeklyCarbs { get; set; }
        public double TotalWeeklyFat { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int MealsPerDay { get; set; }

        public bool IsActive { get; set; }

        public List<UserFoodPlanDaily> UserFoodPlanMeals { get; set; }

        public int IdUser { get; set; }

        [ForeignKey("IdUser")]
        [Required]
        public required User User { get; set; }

        public int IdFoodPlan { get; set; }

        [ForeignKey("IdFoodPlan")]
        [Required]
        public required  FoodPlan FoodPlan { get; set; }

        public int IdGoal { get; set; }

        [ForeignKey("IdGoal")]
        [Required]
        public required Goal Goal { get; set; }

        public int? IdIntolerance { get; set; }

        [ForeignKey("IdIntolerance")]
        public Intolerance? Intolerance { get; set; }

        public DateTime CreationDate { get; set; }
    }

}
