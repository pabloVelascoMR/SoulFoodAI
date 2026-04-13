using SoulFoodAiBack.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoulFoodAiBack.Dtos
{
    public class CreateIngredientDto
    {
        public required string Name { get; set; }
        public required string Category { get; set; }
        public string? SubCategory { get; set; }
        public string? Brand { get; set; }
        public string? ImageUrl { get; set; }
        public int? CreatedByUserId { get; set; }
        public string? Icon { get; set; }
        public double Protein { get; set; }
        public double Carbs { get; set; }
        public double Fat { get; set; }
        public double Kcal { get; set; }
        public string? OpenFoodFactsId { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
