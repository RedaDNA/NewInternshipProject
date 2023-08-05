using Core.Entities;
using APIPart.Modles;

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
        IEnumerable<T> GetAll();
        T GetById(Guid id);
        void Add(T entity);
        bool Update(Guid id, T entity);
        bool Delete(Guid id);
        public Queryable GetUserData(T entity, PagingModel<UserData> inputData);
    }
}
