using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Models
{
    public class UserDiary
    {
        public UserDiary()
        {
            this.CreationDate = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUserDiary { get; set; }

        public int HungerLevel { get; set; }

        public int EnergyLevel { get; set; }

        public int SleepQuality { get; set; }

        public int DietAdherence { get; set; }

        public string? Notes { get; set; }

        public int IdUserFoodPlan { get; set; }

        [ForeignKey("IdUserFoodPlan")]
        [Required]
        public required UserFoodPlan UserFoodPlan { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
