/*using Core.Entities;
using Core.Interfaces;

namespace infrastructure.Services
{
    public class CarService
    {
        private readonly ICarRepository _repository;

        public CarService(ICarRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<Car> GetAllCars()
        {
            return _repository.GetAll();
        }

        public Car GetCarById(Guid id)
        {
            return _repository.GetById(id);
        }

        public void AddCar(Car car)
        {
            _repository.Add(car);
        }

        public void UpdateCar(Car car)
        {
            _repository.Update(car);
        }

        public void DeleteCar(Car car)
        {
            _repository.Delete(car);
        }
    }
}
*/