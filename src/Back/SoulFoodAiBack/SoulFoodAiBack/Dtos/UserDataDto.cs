using SoulFoodAiBack.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Dtos
{
    public class UserDataDto
    {
        public int IdUserData { get; set; }
  
        public required string Gender { get; set; }

        public required int Age { get; set; }

        public required double Height { get; set; }

        public required double Weight { get; set; }

        public int MealsPerDay { get; set; }

        public required int IdUser { get; set; }

        public required int IdFoodPlan { get; set; }

        public required int IdGoal { get; set; }

        public int IdIntolerance { get; set; }
    }
}
