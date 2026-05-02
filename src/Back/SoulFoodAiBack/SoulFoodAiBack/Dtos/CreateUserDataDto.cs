using System.ComponentModel.DataAnnotations;

namespace SoulFoodAiBack.Dtos
{
    public class CreateUserDataDto
    {
        [Required(ErrorMessage = "El género es obligatorio para continuar.")]
        public required string Gender { get; set; }

        [Required(ErrorMessage = "La edad es obligatoria.")]
        [Range(1, 120, ErrorMessage = "Por favor, introduce una edad real (entre 1 y 120).")]
        public required int Age { get; set; }

        [Required(ErrorMessage = "La altura es obligatoria.")]
        [Range(0.5, 3.0, ErrorMessage = "La altura debe ser un valor válido en metros (ej: 1.75).")]
        public required double Height { get; set; }

        [Required(ErrorMessage = "El peso es obligatorio.")]
        [Range(20.0, 300.0, ErrorMessage = "Por favor, introduce un peso válido.")]
        public required double Weight { get; set; }

        public int MealsPerDay { get; set; }

        [Range(1, 5, ErrorMessage = "El nivel de actividad debe ser un valor entre 1 y 5.")]
        public int LevelOfActivity { get; set; }

        [Range(30.0, 200.0, ErrorMessage = "La medida del pecho debe estar entre 30 y 300 cm.")]
        public double? ChestMeasure { get; set; }

        [Range(30.0, 200.0, ErrorMessage = "La medida de la cintura debe estar entre 30 y 300 cm.")]
        public double? WaistMeasure { get; set; }

        [Range(30.0, 200.0, ErrorMessage = "La medida de la cadera debe estar entre 30 y 300 cm.")]
        public double? HipMeasure { get; set; }

        [Range(10.0, 100.0, ErrorMessage = "La medida del bíceps izquierdo debe estar entre 10 y 200 cm.")]
        public double? LeftBicepMeasure { get; set; }

        [Range(10.0, 100.0, ErrorMessage = "La medida del bíceps derecho debe estar entre 10 y 200 cm.")]
        public double? RightBicepMeasure { get; set; }

        [Range(20.0, 150.0, ErrorMessage = "La medida del cuádriceps izquierdo debe estar entre 20 y 250 cm.")]
        public double? LeftCuadricepsMeasure { get; set; }

        [Range(20.0, 150.0, ErrorMessage = "La medida del cuádriceps derecho debe estar entre 20 y 250 cm.")]
        public double? RightCuadricepsMeasure { get; set; }

        public required int IdUser { get; set; }

        public required int IdFoodPlan { get; set; }

        public required int IdGoal { get; set; }

        public List <int>? IdIntolerances { get; set; }
    }
}
