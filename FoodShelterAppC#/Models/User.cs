using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FoodShelterAppC_.Models
{
    public class User : IdentityUser
    {
        [StringLength(100)]
        public string? Name { get; set; }

        public string? Bio { get; set; }

        [StringLength(200)]
        public string? Website { get; set; }

        [StringLength(100)]
        public string? Contact { get; set; }

        [StringLength(20)]
        public string Role { get; set; } = "user";

        [StringLength(200)]
        public string? DonationLink { get; set; }

        // Navigation properties
        public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
        public virtual ICollection<Volunteer> Volunteers { get; set; } = new List<Volunteer>();
        public virtual ICollection<Budget> BudgetEntries { get; set; } = new List<Budget>();
        public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();
        public virtual ICollection<FoodStock> FoodStocks { get; set; } = new List<FoodStock>();
        public virtual ICollection<ShelterLocation> ShelterLocations { get; set; } = new List<ShelterLocation>();
        public virtual ICollection<MealPlan> MealPlans { get; set; } = new List<MealPlan>();
    }
}
