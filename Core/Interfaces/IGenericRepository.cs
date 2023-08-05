using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
namespace Core.Interfaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        IEnumerable<T> GetAll();
        T GetById(Guid id);
        void Add(T entity);
        bool Update(Guid id, T entity);
        bool Delete(Guid id);

        IEnumerable<T> GetCars(Parameters parameters);
    }
}
