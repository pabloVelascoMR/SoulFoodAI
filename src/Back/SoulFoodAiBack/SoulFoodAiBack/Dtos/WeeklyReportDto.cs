namespace SoulFoodAiBack.Dtos
{
    public class WeeklyReportDto
    {
        public int IdUser { get; set; }
        public int HungerLevel { get; set; }
        public int SleepQuality { get; set; }
        public int EnergyLevel { get; set; }
        public int DietAdherence { get; set; }
        public string? Description { get; set; }

        public double? NewWeight { get; set; }
        public UpdateBodyMeasuresDto? NewMeasures { get; set; }
        public bool UseAiAdjustment { get; set; }
        
    }
}
