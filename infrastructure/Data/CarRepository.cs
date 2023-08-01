using Core.Entities;
using Core.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CarRepository : ICarRepository
    {
        private readonly CarRentalContext _context;

        public CarRepository(CarRentalContext context)
        {
            _context = context;
        }

        public IEnumerable<Car> GetAll()
            
        {
            var x = _context.Cars;
            return x;
        }

        public Car GetById(Guid id)
        {
            return _context.Cars.FirstOrDefault(c => c.Id == id);
        }

        public void Add(Car car)
        {
            _context.Cars.Add(car);
            _context.SaveChanges();
        }

        public void Update(Car car)
        {
            _context.Cars.Update(car);
            _context.SaveChanges();
        }

        public void Delete(Car car)
        {
            _context.Cars.Remove(car);
            _context.SaveChanges();
        }
    }
}
