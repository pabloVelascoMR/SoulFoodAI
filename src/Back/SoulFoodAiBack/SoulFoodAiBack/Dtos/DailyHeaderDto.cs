namespace SoulFoodAiBack.Dtos
{
    public class DailyHeaderDto
    {
        public int IdUserFoodPlanDaily { get; set; }
        public string DietName { get; set; } = string.Empty;

        public string DayName { get; set; } = string.Empty;

        public int TargetKcal { get; set; }
        public int RealKcal { get; set; }

        public double TargetProtein { get; set; }
        public double RealProtein { get; set; }

        public double TargetCarbs { get; set; }
        public double RealCarbs { get; set; }

        public double TargetFat { get; set; }
        public double RealFat { get; set; }

        public int MealsPerDay { get; set; }
    }
}