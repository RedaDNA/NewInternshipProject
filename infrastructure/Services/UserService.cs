using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.IServices;
using infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Services
{
    public class UserService : IUserService

    {
        public IUnitOfWork _unitOfWork;


        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<User> AddAsync(User customer)
        {
            var addedUser = await _unitOfWork.Users.AddAsync(customer);
            _unitOfWork.Save();

            return addedUser;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {

            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)

                return false;

            _unitOfWork.Users.DeleteAsync(user.Id);
            var result = _unitOfWork.Save();

            return true;
        }


        public async Task<IEnumerable<User>> GetAllAsync()

        {
            return await _unitOfWork.Users.GetAllAsync();
        }



        public async Task<User> GetByIdAsync(Guid id)
        {

            var customer = await _unitOfWork.Users.GetByIdAsync(id);
            if (customer != null)
            {
                return customer;
            }

            return null;
        }



        public async Task<bool> UpdateAsync(Guid id, User customer)
        {


            var toUpdateUser = await _unitOfWork.Users.GetByIdAsync(id);


            _unitOfWork.Users.UpdateAsync(id, customer);

            var result = _unitOfWork.Save();

            if (result > 0)
                return true;
            else
                return false;

        }
        public IQueryable<User> GetQueryable()

        {
            return _unitOfWork.Users.GetQueryable();
        }
        public async Task<bool> IsExistAsync(Guid id)
        {
            return await _unitOfWork.Users.IsExistAsync(id);



        }
 public       async Task<bool>  IsExistByUsernamePasswordAsync(String username, String password) {

            return await _unitOfWork.Users.IsExistByUsernamePasswordAsync( username,  password);
        }

       
    }




}

