using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecyclingWorld.Models;

namespace RecyclingWorld.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Shipment> Shipments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);   // keep this — Identity needs it

            // Tell SQL Server how to store money values: 18 digits total, 2 after the decimal
            modelBuilder.Entity<Product>().Property(p => p.PricePerKg).HasPrecision(18, 2);
            modelBuilder.Entity<Product>().Property(p => p.PricePerKg500).HasPrecision(18, 2);
            modelBuilder.Entity<Product>().Property(p => p.PricePerKg1000).HasPrecision(18, 2);
            modelBuilder.Entity<OrderDetail>().Property(d => d.Price).HasPrecision(18, 2); // looked up fix that happened duuring migration of database, to prevent errors when trying to save OrderDetails with Price values that have more than 2 decimal places. This ensures that the database schema matches the precision defined in the model, allowing for proper storage and retrieval of monetary values without causing exceptions due to precision mismatches.
        }
    }
}
