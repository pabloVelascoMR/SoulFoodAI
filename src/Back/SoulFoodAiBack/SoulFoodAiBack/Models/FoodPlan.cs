using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Models
{
    public class FoodPlan
    {
        public FoodPlan()
        {
            this.CreationDate = DateTime.Now;

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdFoodPlan { get; set; }

        [Required]
        [StringLength(100)]
        public required string FoodPlanName { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public double ProteinPercent { get; set; }

        public double CarbPercent { get; set; }

        public double FatPercent { get; set; }

        public double VegetableminPercent { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
