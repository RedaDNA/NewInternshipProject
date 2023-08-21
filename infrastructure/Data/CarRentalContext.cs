using Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Core.Entities.identity;

namespace Infrastructure.Data
{
    public class CarRentalContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public CarRentalContext(DbContextOptions<CarRentalContext> options)
         : base(options)
       
    {
    }

    public DbSet<Car> Cars { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<User> AppUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS01;Database=master;Trusted_Connection=True;TrustServerCertificate=True;");
        }

       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {/*
            modelBuilder.Entity<IdentityUser>().ToTable("Users");
            modelBuilder.Entity<IdentityRole>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");*/
            //
             base.OnModelCreating(modelBuilder);
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
