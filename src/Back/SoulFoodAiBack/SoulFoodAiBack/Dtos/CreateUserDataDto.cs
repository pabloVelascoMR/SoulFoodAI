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

        public required int IdUser { get; set; }

        public required int IdFoodPlan { get; set; }

        public required int IdGoal { get; set; }

        public int IdIntolerance { get; set; }
    }
}
