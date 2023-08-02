using Core.Entities;

namespace Core.Interfaces
{
    public interface ICarRepository : IGenericRepository<Car> 
    {
        IEnumerable<Car> GetAvailableCars();
    }
}
