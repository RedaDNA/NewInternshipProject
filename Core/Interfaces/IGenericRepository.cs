using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
       Task<IEnumerable<T>> GetAllAsync();
        Task<T >GetByIdAsync(Guid id);
       Task<T> AddAsync(T entity);
        Task< bool> UpdateAsync(Guid id, T entity);
        Task <bool> DeleteAsync(Guid id);
        IQueryable<T> GetQueryable();

        Task<bool> IsExistAsync(Guid id);
    }
}
