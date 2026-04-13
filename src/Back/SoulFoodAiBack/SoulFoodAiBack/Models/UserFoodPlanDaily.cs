using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Models
{
    public class UserFoodPlanDaily
    {
        public UserFoodPlanDaily()
        {
            this.CreationDate = DateTime.Now;
            IsActive = true;
            FoodPlanDailyRecipes = new List<FoodPlanDailyRecipe>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUserFoodPlanDaily { get; set; }

        public int TargetKcal { get; set; }
        public int RealKcal { get; set; }

        public double TargetProtein { get; set; }
        public double RealProtein { get; set; }

        public double TargetCarbs { get; set; }
        public double RealCarbs { get; set; }

        public double TargetFat { get; set; }
        public double RealFat { get; set; }

        public bool IsActive { get; set; }

        public int IdUser { get; set; }
        [ForeignKey("IdUser")]
        public required User User { get; set; }

        public int IdUserFoodPlanWeek { get; set; }
        [ForeignKey("IdUserFoodPlanWeek")]
        public required UserFoodPlanWeek UserFoodPlanWeek { get; set; }

        public int IdFoodPlan { get; set; }

        [ForeignKey("IdFoodPlan")]
        public required FoodPlan FoodPlan { get; set; }

        public DateTime CreationDate { get; set; }

        public List<FoodPlanDailyRecipe> FoodPlanDailyRecipes { get; set; }

    }
}
