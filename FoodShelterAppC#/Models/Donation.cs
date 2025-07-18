using System.ComponentModel.DataAnnotations;

namespace FoodShelterAppC_.Models
{
    public class Donation
    {
        public int Id { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [StringLength(100)]
        public string? DonorName { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = string.Empty;
        public virtual User? User { get; set; }
    }
}
