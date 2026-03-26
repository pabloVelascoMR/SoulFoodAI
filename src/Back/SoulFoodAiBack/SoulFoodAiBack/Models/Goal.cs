using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Models
{
    public class Goal
    {
        public Goal()
        {
            this.CreationDate = DateTime.Now;

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdGoal { get; set; }

        [Required]
        [StringLength(100)]
        public required string GoalName { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
