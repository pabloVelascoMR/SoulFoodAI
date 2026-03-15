using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Models
{
    public class UserFoodPlanMeal
    {
        public UserFoodPlanMeal()
        {
            this.CreationDate = DateTime.Now;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdFoodPlanMeal { get; set; }

        public double ProteinPercent { get; set; }

        public double CarbPercent { get; set; }

        public double FatPercent { get; set; }

        public double VegetablePercent { get; set; }

        public int IdFoodPlan { get; set; }

        [ForeignKey("IdFoodPlan")]
        [Required]
        public required FoodPlan FoodPlan { get; set; }

        public int IdMeal { get; set; }

        [ForeignKey("IdMeal")]
        [Required]
        public required Meal Meal { get; set; }

        public DateTime CreationDate { get; set; }

    }
}
