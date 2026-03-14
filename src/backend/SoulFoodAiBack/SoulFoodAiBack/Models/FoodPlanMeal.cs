using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Models
{
    public class FoodPlanMeal
    {
        public FoodPlanMeal()
        {
            this.CreationDate = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdFoodPlanMeal { get; set; }

        public double ProteinPercent { get; set; }

        public double CarbPercent { get; set; }

        public double FatPercent { get; set; }

        public double VegetableminPercent { get; set; }

        public int IdFoodPlan { get; set; }

        [ForeignKey("IdFoodPlan")]
        public FoodPlan FoodPlan { get; set; }

        public int IdMeal { get; set; }

        [ForeignKey("IdMeal")]
        public Meal Meal { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
