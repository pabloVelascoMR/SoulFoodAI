using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Models
{
    public class UserFoodPlanWeekIntolerance
    {
        public UserFoodPlanWeekIntolerance()
        {
            this.CreationDate = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUserFoodPlanWeekIntolerance { get; set; }

        public int IdUserFoodPlanWeek { get; set; }
        [ForeignKey("IdUserFoodPlanWeek")]
        public UserFoodPlanWeek? UserFoodPlanWeek { get; set; }

        public int IdIntolerance { get; set; }
        [ForeignKey("IdIntolerance")]
        public Intolerance? Intolerance { get; set; }
        public DateTime CreationDate { get; set; }
    }
}