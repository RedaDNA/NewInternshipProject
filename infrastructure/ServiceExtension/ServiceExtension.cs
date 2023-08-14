using Core.Interfaces;
using infrastructure.Data;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.ServiceExtension
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddDIServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CarRentalContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Server=localhost\\SQLEXPRESS01;Database=master;Trusted_Connection=True;TrustServerCertificate=True;"));
            });
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICarRepository, CarRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();

            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped<IRentalRepository, RentalRepository>();



            return services;
        }
    }
}
