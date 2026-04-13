using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Models
{
    public class UserIntolerance
    {
        public UserIntolerance()
        {
            this.CreationDate = DateTime.Now;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUserIntolerance { get; set; }

        public int IdUser { get; set; }
        [ForeignKey("IdUser")]
        public User? User { get; set; }

        public int IdIntolerance { get; set; }
        [ForeignKey("IdIntolerance")]
        public Intolerance? Intolerance { get; set; }

        public DateTime CreationDate { get; set; }
    }
}