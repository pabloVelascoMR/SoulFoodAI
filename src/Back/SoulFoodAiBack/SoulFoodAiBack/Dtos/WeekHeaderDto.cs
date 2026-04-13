namespace SoulFoodAiBack.Dtos
{
    public class WeeklyHeaderDto
    {
        public int IdUserFoodPlanWeek { get; set; }
        public string DietName { get; set; } = string.Empty;
        public int TotalWeeklyKcal { get; set; }
        public double TargetProteinPercent { get; set; }
        public double TargetCarbsPercent { get; set; }
        public double TargetFatPercent { get; set; }
    }
}
