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
        public  User? User { get; set; }

        public int IdUserFoodPlanDaily { get; set; }
        [ForeignKey("IdUserFoodPlanDaily")]
        public  UserFoodPlanDaily? UserFoodPlanDaily { get; set; }

        public int IdRecipe { get; set; }
        [ForeignKey("IdRecipe")]
        public  Recipe? Recipe { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }
    }
}