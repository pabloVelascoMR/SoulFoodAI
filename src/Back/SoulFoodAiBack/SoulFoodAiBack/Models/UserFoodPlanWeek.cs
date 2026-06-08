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

        public bool IsVisibleInHistory { get; set; }

        public List<UserFoodPlanDaily> UserFoodPlanMeals { get; set; }

        public int IdUser { get; set; }

        [ForeignKey("IdUser")]
        public User? User { get; set; }

        public int IdFoodPlan { get; set; }

        [ForeignKey("IdFoodPlan")]
 
        public FoodPlan? FoodPlan { get; set; }

        public int IdGoal { get; set; }

        [ForeignKey("IdGoal")]
      
        public Goal? Goal { get; set; }

        public List<UserFoodPlanWeekIntolerance> UserFoodPlanWeekIntolerances { get; set; } = new List<UserFoodPlanWeekIntolerance>();

        public DateTime CreationDate { get; set; }
    }

}
