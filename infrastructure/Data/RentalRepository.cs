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
        public async Task<bool> IsDriverExistInAsync(Guid driverId)
        {
            return await _context.Set<Rental>().AnyAsync(c => c.DriverId == driverId);

        }
        public  async Task<bool> IsCarRentedAsync(Guid carId, DateTime startDate, DateTime endDate) {
            var existingRental =  await _context.Set<Rental>()
            .FirstOrDefaultAsync(r =>
                r.CarId == carId &&
                ((r.StartDate >= startDate && r.StartDate <= endDate) ||
                (r.EndDate >= startDate && r.EndDate <= endDate) ||
                (r.StartDate <= startDate && r.EndDate >= endDate)));

            return existingRental != null;

        }

        public async Task<bool> IsDriverBusy(Guid driverId, DateTime startDate, DateTime endDate)
        {
            var existingRental = await _context.Set<Rental>()
         .FirstOrDefaultAsync(r =>
             r.DriverId == driverId &&
             ((r.StartDate >= startDate && r.StartDate <= endDate) ||
             (r.EndDate >= startDate && r.EndDate <= endDate) ||
             (r.StartDate <= startDate && r.EndDate >= endDate)));

            return existingRental != null;
        }
    }
}
