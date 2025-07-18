using System.ComponentModel.DataAnnotations;

namespace FoodShelterAppC_.Models
{
    public class FoodStock
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        public decimal Quantity { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        public DateTime? ExpirationDate { get; set; }

        public int MinimumStock { get; set; }

        [Required]
        [StringLength(20)]
        public string Unit { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        public virtual User? User { get; set; }

        public virtual ICollection<MealIngredient> MealIngredients { get; set; } = new List<MealIngredient>();
    }
}
