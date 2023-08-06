using Core.Entities;
using Core.Interfaces;
using Core.Models;
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

        public IEnumerable<T> GetAll()

        {       
                return _context.Set<T>().ToList();
        }
        public IQueryable<T> GetQueryable()

        {
            return _context.Set<T>().AsQueryable();
        }
        public PagedList<T> GetCarsPages(Parameters parameters)
        {
            return PagedList<T>.ToPagedList(_context.Set<T>().OrderBy(on => on.Id),
                parameters.PageNumber,
                parameters.PageSize);
        }
        public   IEnumerable<T> GetCars(Parameters parameters)
        {

            return _context.Set<T>()
            .OrderBy(on => on.Id)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToList();

        }
        public T GetById(Guid id)
        {
            return _context.Set<T>().FirstOrDefault(c => c.Id == id);
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
        }

        public bool Update(Guid id,T entity)
        {
            var oldEntity= _context.Set<T>().FirstOrDefault(c => c.Id == id);
            if (oldEntity == null)
            {
                return false;
            }
          else
            {
                oldEntity = entity;
                _context.Update(oldEntity);
                _context.SaveChanges();
                return true;
            }
        }

        public bool Delete(Guid id)
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
