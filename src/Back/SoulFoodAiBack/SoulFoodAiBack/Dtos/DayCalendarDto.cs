using System;
using System.Collections.Generic;

namespace SoulFoodAiBack.Dtos
{
    public class DayCalendarDto
    {
        public int IdUserFoodPlanDaily { get; set; }
        public string DayName { get; set; } = string.Empty; 
        public string DateNumber { get; set; } = string.Empty; 
        public DateTime FullDate { get; set; }
        public List<DailyRecipeDto> AssignedRecipes { get; set; } = new List<DailyRecipeDto>();
    }
}