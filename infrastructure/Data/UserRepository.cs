using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Data
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly CarRentalContext _context;
        public UserRepository(CarRentalContext context) : base(context)
        {
            _context = context;

        }

        public async Task<bool> IsExistByUsernamePasswordAsync(string username, string password)
        {
            return  await _context.Set<User>().AnyAsync(u => u.UserName == username && u.Password == password);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Set<User>().AnyAsync(u => u.UserName == username);

        }
    }
}
