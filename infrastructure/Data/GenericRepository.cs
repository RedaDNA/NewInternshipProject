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

    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity

    {
        private readonly CarRentalContext _context;
        public GenericRepository(CarRentalContext context)
        {
            _context = context;
        }
        public  IQueryable<T> GetQueryable()

        {
            return _context.Set<T>().AsQueryable();
        }
        public async Task<IEnumerable<T>> GetAllAsync()

        {       
                return await   _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<bool> IsExistAsync(Guid id) 
        {
            return await _context.Set<T>().AnyAsync(c => c.Id == id);
        }
        public async Task<T> AddAsync(T entity)
        {
             _context.Set<T>().Add(entity);

         await   _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(Guid id,T entity)
        {
            var oldEntity= await _context.Set<T>().FirstOrDefaultAsync(c => c.Id == id);
            if (oldEntity == null)
            {
                return false;
            }
          else
            {
              
                _context.Update(oldEntity).CurrentValues.SetValues(entity); ;
              await  _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var toRemoveEntity = await _context.Set<T>().FirstOrDefaultAsync(c => c.Id == id);
            if (toRemoveEntity == null)
            {
                return false;
            }
            else
            {
                _context.Remove(toRemoveEntity);
                await _context.SaveChangesAsync();
                return true;
            }
          
        }

       
    }
}
