using System.ComponentModel.DataAnnotations;

namespace FoodShelterAppC_.Models
{
    public class ShelterLocation
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        public DateTime Date { get; set; }

        public float? Latitude { get; set; }
        public float? Longitude { get; set; }

        public string UserId { get; set; } = string.Empty;
        public virtual User? User { get; set; }
    }
}
