using System.ComponentModel.DataAnnotations;

namespace FoodShelterAppC_.Models
{
    public class Budget
    {
        public int Id { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string UserId { get; set; } = string.Empty;
        public virtual User? User { get; set; }
    }
}
