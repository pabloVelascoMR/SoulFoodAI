using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Models
{
    public class Intolerance
    {
        public Intolerance()
        {
            this.CreationDate = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdIntolerance { get; set; }

        [Required]
        [StringLength(100)]
        public required string IntoleranceName { get; set; }

        public DateTime CreationDate { get; set; }
    }
}

