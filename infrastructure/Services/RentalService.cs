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
    public class RentalService : IRentalService
    {
        public IUnitOfWork _unitOfWork;


        public RentalService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Rental> AddAsync(Rental rental)
        {
            await _unitOfWork.Rentals.AddAsync(rental);
           _unitOfWork.Save();
          
                
            return rental;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {

            try
            {
                await _unitOfWork.Rentals.DeleteAsync(id);
                var result = _unitOfWork.Save();

            }
            catch (Exception ex)
            {
                return (false);


            }


            return (false );
        }


        public async Task<IEnumerable<Rental>> GetAllAsync()

        {
            return await _unitOfWork.Rentals.GetAllAsync();
        }



        public async Task<Rental> GetByIdAsync(Guid id)
        {

            var rental = await _unitOfWork.Rentals.GetByIdAsync(id);
            if (rental != null)
            {
                return rental;
            }

            return null;
        }



        public async Task<bool> UpdateAsync(Guid id, Rental rental)
        {

          

         await   _unitOfWork.Rentals.UpdateAsync(id, rental);

            var result = _unitOfWork.Save();

            if (result > 0)
                return true;
            else
                return false;

        }


        public IQueryable<Rental> GetQueryable()

        {
            return _unitOfWork.Rentals.GetQueryable();
        }
        public async Task<bool> IsExistAsync(Guid id)
        {
            return await _unitOfWork.Rentals.IsExistAsync(id);



        }
    }
}
