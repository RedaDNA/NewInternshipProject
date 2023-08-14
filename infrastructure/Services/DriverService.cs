using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Services
{
    public class DriverService : IDriverService
    {
        public IUnitOfWork _unitOfWork;


        public DriverService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Driver> AddAsync(Driver driver)
        {
            await _unitOfWork.Drivers.AddAsync(driver);
            _unitOfWork.Save();


            return driver;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {

            var driver = await _unitOfWork.Drivers.GetByIdAsync(id);
            if (driver == null)

                return false;

            _unitOfWork.Drivers.DeleteAsync(driver.Id);
            var result = _unitOfWork.Save();

            return true;
        }


        public async Task<IEnumerable<Driver>> GetAllAsync()

        {
            return await _unitOfWork.Drivers.GetAllAsync();
        }



        public async Task<Driver> GetByIdAsync(Guid id)
        {

            var driver = await _unitOfWork.Drivers.GetByIdAsync(id);
            if (driver != null)
            {
                return driver;
            }

            return null;
        }



        public async Task<bool> UpdateAsync(Guid id, Driver driver)
        {
              var toUpdateDriver = await _unitOfWork.Drivers.IsExistAsync(id);
      
                    _unitOfWork.Drivers.UpdateAsync(id, driver);

                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
            
           
        }
        public IQueryable<Driver> GetQueryable()

        {
            return _unitOfWork.Drivers.GetQueryable();
        }
        public async Task<bool> IsExistAsync(Guid id)
        {
            return await _unitOfWork.Drivers.IsExistAsync(id);



        }

        public async Task<bool> IsAvailableAsync(Guid id)
        {
            return await _unitOfWork.Drivers.IsAvailableAsync(id);
        }
    }

}

