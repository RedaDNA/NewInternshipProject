﻿using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Services
{
    public class CustomerService :ICustomerService
    {
        public IUnitOfWork _unitOfWork;


        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Customer> AddAsync(Customer customer)
        {
          var addedCustomer=  await _unitOfWork.Customers.AddAsync(customer);
            _unitOfWork.Save();

            return addedCustomer;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {

            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer == null)

                return false;

            await _unitOfWork.Customers.DeleteAsync(customer.Id);
            var result = _unitOfWork.Save();

            return true;
        }


        public async Task<IEnumerable<Customer>> GetAllAsync()

        {
            return await _unitOfWork.Customers.GetAllAsync();
        }



        public async Task<Customer> GetByIdAsync(Guid id)
        {

            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
                return customer;
            
       
        }



        public async Task<bool> UpdateAsync(Guid id, Customer customer)
        {
           
            
                var toUpdateCustomer = await _unitOfWork.Customers.GetByIdAsync(id);
              

                    await    _unitOfWork.Customers.UpdateAsync(id, customer);

                    var result = _unitOfWork.Save();

                    if (result > 0)
                        return true;
                    else
                        return false;
         
        }
        public IQueryable<Customer> GetQueryable()

        {
            return _unitOfWork.Customers.GetQueryable();
        }
        public async Task<bool> IsExistAsync(Guid id)
        {
            return await _unitOfWork.Customers.IsExistAsync(id);



        }
    }




}
