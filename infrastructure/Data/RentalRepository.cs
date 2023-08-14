using Core.Entities;
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
    public class RentalRepository : GenericRepository<Rental>, IRentalRepository
    {
        private readonly CarRentalContext _context;
        public RentalRepository(CarRentalContext context) : base(context)
        {
            _context = context;

        }

        public Task<bool> IsCustomerExistInAsync(Guid customerId)
        {
            return _context.Set<Rental>().AnyAsync(c => c.CustomerId == customerId);
        }

       public Task<bool> IsCarExistInAsync(Guid carId)
        {
            return  _context.Set<Rental>().AnyAsync(c => c.CarId == carId);
           
        }
        public Task<bool> IsDriverExistInAsync(Guid driverId)
        {
            return _context.Set<Rental>().AnyAsync(c => c.DriverId == driverId);

        }

    }
}
