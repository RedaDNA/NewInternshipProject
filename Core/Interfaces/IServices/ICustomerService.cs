using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.IServices
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(Guid id);
        Task<Customer> AddAsync(Customer car);
        Task<bool> UpdateAsync(Guid id, Customer car);
        Task<bool> DeleteAsync(Guid id);
        IQueryable<Customer> GetQueryable();

        Task<bool> IsExistAsync(Guid id);
    }
}
