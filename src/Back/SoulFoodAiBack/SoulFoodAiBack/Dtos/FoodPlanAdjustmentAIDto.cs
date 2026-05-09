namespace SoulFoodAiBack.Dtos
{
    public class FoodPlanAdjustmentAIDto
    {
        public string AnalysisMessage { get; set; } = string.Empty;
        public int SuggestedKcal { get; set; }
        public double SuggestedProteinPercentage { get; set; }
        public double SuggestedCarbsPercentage { get; set; }
        public double SuggestedFatPercentage { get; set; }
    }
}
