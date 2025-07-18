using System.ComponentModel.DataAnnotations;

namespace FoodShelterAppC_.Models
{
    public class Volunteer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(120)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? Availability { get; set; }

        [StringLength(200)]
        public string? Skills { get; set; }

        public string? Notes { get; set; }

        public string UserId { get; set; } = string.Empty;
        public virtual User? User { get; set; }
    }
}
