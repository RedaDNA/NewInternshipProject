﻿using APIPart.DTOs.CustomerDtos;
using APIPart.DTOs;
using APIPart.ErrorHandling;
using AutoMapper;
using Core.Entities;
using Core.enums;
using Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIPart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerControllerUOW : Controller
    {
        private readonly IMapper _mapper;
        private ICustomerService _customerService;

        public CustomerControllerUOW(ICarService carService, IMapper mapper, ICustomerService customerService )
        {
            _mapper = mapper;
            _customerService = customerService;
        }
        [Route("GetCustomers")]
        [HttpGet]
        public async Task<ApiResponse> GetCustomersAsync([FromQuery] ListRequestDto listRequestDto)
        {
            var searchWord = listRequestDto.SearchWord.ToLower();

            var query = _customerService.GetQueryable()
                .Where(c =>
                    c.Name.ToLower().Contains(searchWord) ||
                    c.Phone.ToLower().Contains(searchWord) ||
                    c.Email.ToLower().Contains(searchWord)
                );
            var count = await query.CountAsync();
            if (listRequestDto.SortingType == SortingType.asc)
            {
                query = query.OrderBy(c => c.Name);
            }
            else if (listRequestDto.SortingType == SortingType.desc)
            {
                query = query.OrderByDescending(c => c.Name);
            }
            var pageIndex = listRequestDto.PageNumber - 1;
            var pageSize = listRequestDto.PageSize;

            query = query.Skip(pageIndex * pageSize).Take(pageSize);
            var customers = await query.ToListAsync();

            var customerPaginationDto = _mapper.Map<CustomerPaginationDto>(customers);
            customerPaginationDto.Count = count;
            return new ApiOkResponse(customerPaginationDto);
        }

        [HttpGet]
        public async Task<ApiResponse> GetListAsync()
        {
            var customers = await _customerService.GetAllAsync();

            IEnumerable<CustomerListDto>? customerListDto = _mapper.Map<IEnumerable<CustomerListDto>>(customers);
            return new ApiOkResponse(customerListDto);
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse> GetAsync(Guid id)
        {
            var customer = await _customerService.GetByIdAsync(id);

            CustomerDto customerDto = _mapper.Map<CustomerDto>(customer);
            return new ApiOkResponse(customerDto);
        }

        [HttpPost]
        public async Task<ApiResponse> CreateAsync(CreateCustomerDto createCustomerDto)
        {
            Customer toCreateCustomer = _mapper.Map<Customer>(createCustomerDto);
            await _customerService.AddAsync(toCreateCustomer);
            return new ApiOkResponse(createCustomerDto);
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse> UpdateAsync(Guid id, UpdateCustomerDto updateCustomerDto)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
            {
                return null;
            }
            else
            {
                var newCustomer = _mapper.Map<Customer>(updateCustomerDto);
                await _customerService.UpdateAsync(id, newCustomer);
                return new ApiOkResponse(updateCustomerDto);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse> DeleteAsync(Guid id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
            {
                return null;
            }
            await _customerService.DeleteAsync(id);

            CustomerDto customerDto = _mapper.Map<CustomerDto>(customer);

            return new ApiOkResponse(customerDto);
        }
    }
}
