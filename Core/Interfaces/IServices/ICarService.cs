using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.IServices
{
    public interface ICarService
        
    {

        Task<IEnumerable<Car>> GetAllAsync();
        Task<Car> GetByIdAsync(Guid id);
        Task<Car> AddAsync(Car car);
        Task<bool> UpdateAsync(Guid id, Car car);
        Task<bool> DeleteAsync(Guid id);
        IQueryable<Car> GetQueryable();

        Task<bool> IsExistAsync(Guid id);

    }
}
