namespace SoulFoodAiBack.Dtos
{
    public class UpdateBodyMeasuresDto
    {
        public int IdUser { get; set; }
        public double ChestMeasure { get; set; }
        public double WaistMeasure { get; set; }
        public double HipMeasure { get; set; }
        public double LeftBicepMeasure { get; set; }
        public double RightBicepMeasure { get; set; }
        public double LeftCuadricepsMeasure { get; set; }
        public double RightCuadricepsMeasure { get; set; }
    }
}
