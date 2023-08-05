using Core.Entities;
using Core.Interfaces;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Extentions;
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

        public void UpdateCar(Guid id , Car car)
        {
            _repository.Update(id, car);
        }

        public void DeleteCar(Guid id )
        {
            _repository.Delete(id);
        }

        public async Task<PagingModel<Car>> GetUser(Car car) {
          var query =  _repository.GetUserData(car);
           var result = await query.GetPagedResult(inputData.CurrentPage, inputData.RowsPerPage, inputData.OrderByData, false);


        }
    }
}
