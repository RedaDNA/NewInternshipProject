using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data
{
    public class CarRentalContext: DbContext 
    {
        public CarRentalContext(DbContextOptions options)
        : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS01;Database=master;Trusted_Connection=True;TrustServerCertificate=True;");
        }

       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            //
            modelBuilder.Entity<Car>()
            .HasOne(c => c.Driver)
            .WithMany(d => d.Cars)
            .HasForeignKey(c => c.DriverId)
            .OnDelete(DeleteBehavior.Restrict).IsRequired(false);

            modelBuilder.Entity<Rental>()
          .HasOne(r => r.Driver)
          .WithMany(d => d.Rentals)
          .HasForeignKey(r => r.DriverId)
          .OnDelete(DeleteBehavior.Restrict).IsRequired(false);


            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Car)
                .WithMany(c => c.Rentals)
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Restrict).IsRequired(true); ;

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Customer)
                .WithMany(c => c.Rentals)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict).IsRequired(true); ;

            modelBuilder.Entity<Driver>()
          .HasOne(d => d.ReplacementDriver)
          .WithMany()
          .HasForeignKey(rd => rd.Id)
          .OnDelete(DeleteBehavior.Restrict).IsRequired(false);
        }
    }
}
