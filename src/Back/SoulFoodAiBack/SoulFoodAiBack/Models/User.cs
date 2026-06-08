using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Models
{
    public class User
    {
        public User()
        {
            this.CreationDate = DateTime.Now;
            this.UserIngredients = new List<UserIngredient>();
            this.UserFoodPlans = new List<UserFoodPlanWeek>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUser { get; set; }

        [Required]
        [StringLength(50)]
        public required string UserName { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string PasswordHash { get; set; }


        public UserData? UserData { get; set; }

        public List<UserIngredient> UserIngredients { get; set; }

        public List<UserFoodPlanWeek> UserFoodPlans { get; set; }

        public DateTime CreationDate { get; set; }

    }
}
