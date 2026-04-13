using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Models
{
    public class FoodPlanDailyRecipe
    {
        public FoodPlanDailyRecipe()
        {
            CreationDate = DateTime.Now;
            IsActive = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdFoodPlanDailyRecipe { get; set; }

        public int IdUser { get; set; }
        [ForeignKey("IdUser")]
        public required User User { get; set; }

        public int IdUserFoodPlanDaily { get; set; }
        [ForeignKey("IdUserFoodPlanDaily")]
        public required UserFoodPlanDaily UserFoodPlanDaily { get; set; }

        public int IdRecipe { get; set; }
        [ForeignKey("IdRecipe")]
        public required Recipe Recipe { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
    }
}