using System.Collections.Generic;

namespace SoulFoodAiBack.Dtos
{
  
    public class WeekCalendarDto
    {
        public int IdUserFoodPlanWeek { get; set; }
        public int MealsPerDay { get; set; } 

        public List<DayCalendarDto> Days { get; set; } = new List<DayCalendarDto>();
    }
}