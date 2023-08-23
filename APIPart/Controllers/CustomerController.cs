using APIPart.DTOs.CustomerDtos;
using APIPart.DTOs;
using APIPart.ErrorHandling;
using AutoMapper;
using Core.Entities;
using Core.enums;
using Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using infrastructure.Services;
using APIPart.DTOs.CarDtos;

namespace APIPart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : Controller
    {
        private readonly IMapper _mapper;
        private ICustomerService _customerService;
        private readonly IRentalService _rentalService;

        public CustomerController(ICarService carService, IMapper mapper, ICustomerService customerService, IRentalService rentalService )
        {
            _mapper = mapper;
            _customerService = customerService;
            _rentalService = rentalService;
        }
        [Route("GetCustomers")]
        [HttpGet]
        public async Task<ApiResponse> GetCustomersAsync([FromQuery] CustomerRequestDto customerRequestDto)
        {

            var query = _customerService.GetQueryable();
            if (!string.IsNullOrEmpty(customerRequestDto.SearchWord))
            {
                var searchWord = customerRequestDto.SearchWord.ToLower();

                query = query
                    .Where(c =>
                        c.Name.ToLower().Contains(searchWord) ||
                        c.Phone.ToLower().Contains(searchWord) ||
                        c.Email.ToLower().Contains(searchWord)
                    );
            }
            var count = await query.CountAsync();
            var columnName = customerRequestDto.SortingColumn.ToLower();
            switch (columnName)
            {
                case "name":
                    query = customerRequestDto.SortingType == "asc"
                        ? query.OrderBy(c => c.Name)
                        : query.OrderByDescending(c => c.Name);
                    break;
                case ("email"):
                    query = customerRequestDto.SortingType == "asc"
                        ? query.OrderBy(c => c.Email)
                        : query.OrderByDescending(c => c.Email);
                    break;
                case ("phone"):
                    query = customerRequestDto.SortingType == "asc"
                ? query.OrderBy(c => c.Phone)
                : query.OrderByDescending(c => c.Phone);
                    break;
        
                default:

                    query = query.OrderBy(c => c.Name);
                    break;
            }
       
            var pageIndex = customerRequestDto.PageNumber - 1;
            var pageSize = customerRequestDto.PageSize;

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
                newCustomer.Id = id;

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
            var customerUsedInRental = await _rentalService.IsCustomerExistInAsync(id);
            if (customerUsedInRental)
            {
                return new ApiResponse(404, "Cannot delete the Customer, Customer is already used in rental record");


            }
            await _customerService.DeleteAsync(id);

            CustomerDto customerDto = _mapper.Map<CustomerDto>(customer);

            return new ApiOkResponse(customerDto);
        }
    }
}
