using Core.Entities;
using Core.Interfaces;
using Core.Models;
using infrastructure.Data;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CarRepository : GenericRepository<Car> ,ICarRepository
    {
        private readonly CarRentalContext _context;
        public CarRepository(CarRentalContext context) : base (context)
        {
            _context = context;

        }
        public IEnumerable<Car> GetAvailableCars()
        {
            return _context.Cars.AsEnumerable().Where(c =>c.IsAvailable ==true);
        }

       
    }
}
    

