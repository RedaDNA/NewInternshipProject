using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Data
{
    internal class UnitOfWork : IUnitOfWork

    {
        private readonly CarRentalContext _context;
        public ICarRepository Cars { get; }
        public ICustomerRepository Customers { get; }

        public IDriverRepository  Drivers{ get; }
        public IRentalRepository Rentals { get; }

        public UnitOfWork(CarRentalContext context,
                            ICarRepository carRepository,
                         ICustomerRepository customerRepository   , IDriverRepository driverRepository,IRentalRepository rentalRepository
                            )
        {
            _context = context;
            Cars = carRepository;
            Drivers = driverRepository;
            Customers = customerRepository;
            Rentals
= rentalRepository;        }
        public int Save()
        {
            return _context.SaveChanges();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
