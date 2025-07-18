using FoodShelterAppC_.Models;
using System.Collections.Generic;
using System.Linq;

namespace FoodShelterAppC_.Models.ViewModels
{
    public class DashboardViewModel
    {
        public List<FoodStock> FoodStocks { get; set; } = new();
        public List<ShelterLocation> ShelterLocations { get; set; } = new();
        public List<Note> Notes { get; set; } = new();
        public List<Volunteer> Volunteers { get; set; } = new();
        public List<Budget> BudgetEntries { get; set; } = new();
        public List<Donation> Donations { get; set; } = new();

        // Computed properties
        public int LowStockCount
        {
            get { return FoodStocks.Count(f => f.Quantity <= f.MinimumStock); }
        }

        public int ExpiringCount
        {
            get
            {
                var today = DateTime.Today;
                var weekFromNow = today.AddDays(7);
                return FoodStocks.Count(f => f.ExpirationDate.HasValue &&
                    f.ExpirationDate.Value <= weekFromNow &&
                    f.ExpirationDate.Value >= today);
            }
        }

        public decimal TotalDonations
        {
            get { return Donations.Sum(d => d.Amount); }
        }

        public int ActiveVolunteersCount
        {
            get { return Volunteers.Count; }
        }

        public int ShelterLocationsCount
        {
            get { return ShelterLocations.Count; }
        }
    }
}