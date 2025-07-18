using System.ComponentModel.DataAnnotations;

namespace FoodShelterAppC_.Models
{
    public class MealIngredient
    {
        public int Id { get; set; }

        public int MealPlanId { get; set; }
        public virtual MealPlan MealPlan { get; set; } = null!;

        public int FoodStockId { get; set; }
        public virtual FoodStock FoodStock { get; set; } = null!;

        [Required]
        public decimal QuantityPerServing { get; set; }
    }
}
