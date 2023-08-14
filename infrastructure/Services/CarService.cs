using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Services
{
    public class CarService : ICarService
    {
        public IUnitOfWork _unitOfWork;


        public CarService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Car> AddAsync(Car car)
        {
            await _unitOfWork.Cars.AddAsync(car);
            _unitOfWork.Save();
            return car;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
          
                    
                _unitOfWork.Cars.DeleteAsync(id);
                var result = _unitOfWork.Save();

            return (result>0);    
                }
            
        
        public async Task<IEnumerable<Car>> GetAllAsync()

        {
            return await _unitOfWork.Cars.GetAllAsync();
        }

     

        public async Task< Car> GetByIdAsync(Guid id)
        {
           
                var car = await _unitOfWork.Cars.GetByIdAsync(id);
                if (car != null)
                {
                    return car;
                }

            return null;
        }

     

        public async Task<bool> UpdateAsync(Guid id,Car car)
        {
            
                var toUpdateCar = await _unitOfWork.Cars.GetByIdAsync(id);
              
                    _unitOfWork.Cars.UpdateAsync(id,car);

                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
               
            }
        
        
        public IQueryable<Car> GetQueryable()

        {
            return _unitOfWork.Cars.GetQueryable();
        }
        public async Task<bool> IsExistAsync(Guid id)
        {
            return await _unitOfWork.Cars.IsExistAsync(id);
                
                
    
        }
    }



}
