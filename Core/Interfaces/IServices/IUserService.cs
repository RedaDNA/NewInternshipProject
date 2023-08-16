using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.IServices
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(Guid id);
        Task<User> AddAsync(User user);
        Task<bool> UpdateAsync(Guid id, User user);
        Task<bool> DeleteAsync(Guid id);
        IQueryable<User> GetQueryable();

        Task<bool> IsExistAsync(Guid id);

        Task<bool> IsExistByUsernamePasswordAsync(String username, String password);
    }
}
