﻿using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.IServices
{
    public interface IRentalService
    {
        Task<IEnumerable<Rental>> GetAllAsync();
        Task<Rental> GetByIdAsync(Guid id);
        Task<Rental> AddAsync(Rental rental);
        Task<bool> UpdateAsync(Guid id, Rental rental);
        Task<bool> DeleteAsync(Guid id);
        IQueryable<Rental> GetQueryable();

        Task<bool> IsExistAsync(Guid id);
        Task<bool> IsCarExistInAsync(Guid carId);
        Task<bool> IsCustomerExistInAsync(Guid customerId);
        Task<bool> IsDriverExistInAsync(Guid id);
        Task<bool> IsCarRentedAsync(Guid carId, DateTime startDate, DateTime endDate);
        Task<bool> IsDriverBusy(Guid driverId, DateTime startDate, DateTime endDate);
    }
    
}
