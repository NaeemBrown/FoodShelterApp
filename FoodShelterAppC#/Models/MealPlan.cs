using System.ComponentModel.DataAnnotations;

namespace FoodShelterAppC_.Models
{
    public class MealPlan
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public int Servings { get; set; }

        public bool IsPrepared { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = string.Empty;
        public virtual User? User { get; set; }

        public virtual ICollection<MealIngredient> Ingredients { get; set; } = new List<MealIngredient>();
    }
}
