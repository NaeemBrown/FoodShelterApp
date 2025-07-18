using System.ComponentModel.DataAnnotations;

namespace FoodShelterAppC_.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = string.Empty;
        public virtual User? User { get; set; }
    }
}
