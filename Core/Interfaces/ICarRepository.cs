using Core.Entities;
using Core.Models;

namespace Core.Interfaces
{
    public interface ICarRepository : IGenericRepository<Car> 
    {
        IEnumerable<Car> GetAvailableCars();
      
    }

  
}
