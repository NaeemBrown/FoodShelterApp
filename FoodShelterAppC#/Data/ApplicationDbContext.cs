using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using FoodShelterAppC_.Models;


namespace FoodShelterAppC_.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<FoodStock> FoodStocks { get; set; }
        public DbSet<ShelterLocation> ShelterLocations { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<MealPlan> MealPlans { get; set; }
        public DbSet<MealIngredient> MealIngredients { get; set; }
        public DbSet<Donation> Donations { get; set; }
        public DbSet<Budget> Budgets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for Budget and Donation
            modelBuilder.Entity<Budget>()
                .Property(b => b.Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Donation>()
                .Property(d => d.Amount)
                .HasPrecision(18, 2);

            // Configure Budget relationship
            modelBuilder.Entity<Budget>()
                .HasOne(b => b.User)
                .WithMany(u => u.BudgetEntries)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Donation relationship
            modelBuilder.Entity<Donation>()
                .HasOne(d => d.User)
                .WithMany(u => u.Donations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure FoodStock relationship
            modelBuilder.Entity<FoodStock>()
                .HasOne(f => f.User)
                .WithMany(u => u.FoodStocks)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure MealPlan relationship
            modelBuilder.Entity<MealPlan>()
                .HasOne(m => m.User)
                .WithMany(u => u.MealPlans)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure MealIngredients relationships
            modelBuilder.Entity<MealIngredient>()
                .HasOne(mi => mi.FoodStock)
                .WithMany(fs => fs.MealIngredients)
                .HasForeignKey(mi => mi.FoodStockId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<MealIngredient>()
                .HasOne(mi => mi.MealPlan)
                .WithMany(mp => mp.Ingredients)
                .HasForeignKey(mi => mi.MealPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Note relationship
            modelBuilder.Entity<Note>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notes)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ShelterLocation relationship
            modelBuilder.Entity<ShelterLocation>()
                .HasOne(s => s.User)
                .WithMany(u => u.ShelterLocations)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Volunteer relationship
            modelBuilder.Entity<Volunteer>()
                .HasOne(v => v.User)
                .WithMany(u => u.Volunteers)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure string length constraints
            modelBuilder.Entity<FoodStock>()
                .Property(f => f.ItemName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<FoodStock>()
                .Property(f => f.Category)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<FoodStock>()
                .Property(f => f.Unit)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<Volunteer>()
                .Property(v => v.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Volunteer>()
                .Property(v => v.Email)
                .HasMaxLength(120)
                .IsRequired();

            modelBuilder.Entity<Volunteer>()
                .Property(v => v.Phone)
                .HasMaxLength(20);

            modelBuilder.Entity<Volunteer>()
                .Property(v => v.Availability)
                .HasMaxLength(200);

            modelBuilder.Entity<Volunteer>()
                .Property(v => v.Skills)
                .HasMaxLength(200);

            modelBuilder.Entity<ShelterLocation>()
                .Property(s => s.Address)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<MealPlan>()
                .Property(m => m.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Donation>()
                .Property(d => d.DonorName)
                .HasMaxLength(100);

            modelBuilder.Entity<Budget>()
                .Property(b => b.Description)
                .HasMaxLength(200);

            modelBuilder.Entity<MealIngredient>()
                .Property(mi => mi.QuantityPerServing)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<FoodStock>()
                .Property(fs => fs.Quantity)
                .HasColumnType("decimal(18,2)");
        }
    }
}