using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.IServices
{
    public interface IDriverService
    {
        Task<IEnumerable<Driver>> GetAllAsync();
        Task<Driver> GetByIdAsync(Guid id);
        Task<Driver> AddAsync(Driver car);
        Task<bool> UpdateAsync(Guid id, Driver car);
        Task<bool> DeleteAsync(Guid id);
        IQueryable<Driver> GetQueryable();

        Task<bool> IsExistAsync(Guid id);
    }
}
