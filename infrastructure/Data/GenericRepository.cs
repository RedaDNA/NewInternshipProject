using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
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
        public IQueryable<T> GetQueryable()

        {
            return _context.Set<T>().AsQueryable();
        }
        public async Task<IEnumerable<T>> GetAllAsync()

        {       
                return   _context.Set<T>().ToList();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return _context.Set<T>().FirstOrDefault(c => c.Id == id);
        }

        public async Task<T> AddAsync(T entity)
        {
            _context.Set<T>().Add(entity);

            _context.SaveChanges();
            return entity;
        }

        public async Task<bool> UpdateAsync(Guid id,T entity)
        {
            var oldEntity=  _context.Set<T>().FirstOrDefault(c => c.Id == id);
            if (oldEntity == null)
            {
                return false;
            }
          else
            {
              
                _context.Update(oldEntity).CurrentValues.SetValues(entity); ;
                _context.SaveChanges();
                return true;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var toRemoveEntity = _context.Set<T>().FirstOrDefault(c => c.Id == id);
            if (toRemoveEntity == null)
            {
                return false;
            }
            else
            {
                _context.Remove(toRemoveEntity);
                _context.SaveChanges();
                return true;
            }
          
        }

       
    }
}
