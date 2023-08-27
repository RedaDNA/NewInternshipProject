using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRentalRepository : IGenericRepository<Rental>
    {

      public  Task<bool>  IsCarExistInAsync(Guid id);
        public Task<bool> IsCustomerExistInAsync(Guid customerId);
        public Task<bool> IsDriverExistInAsync(Guid driverId);

        public  Task<bool> IsCarRentedAsync(Guid carId, DateTime startDate, DateTime endDate);

        Task<bool> IsDriverBusy(Guid driverId, DateTime startDate, DateTime endDate);

    }
}
