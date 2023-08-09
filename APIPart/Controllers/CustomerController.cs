
using APIPart.DTOs;
using APIPart.DTOs.CustomerDtos;
using AutoMapper;
using Core.Entities;
using Core.enums;
using Core.Interfaces;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace APIPart.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : Controller
    {
        private IGenericRepository<Customer> _customerRepository;

        private readonly IMapper _mapper;
        public CustomerController(IGenericRepository<Customer> customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }
        [Route("GetCustomers")]

        [HttpGet]
        public CustomerPaginationDto GetCustomers([FromQuery] ListRequestDto listRequestDto)
        {
            var searchWord = listRequestDto.SearchWord.ToLower();

            var query = _customerRepository.GetQueryable()
                .Where(c =>
                    c.Name.ToLower().Contains(searchWord) ||
                    c.Phone.ToLower().Contains(searchWord) ||
                    c.Email.ToLower().Contains(searchWord) 
                );
            var count = query.Count();
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
            var customers = query.ToList();

            var customerPaginationDto = _mapper.Map<CustomerPaginationDto>(customers);
            customerPaginationDto.Count = count;
            return customerPaginationDto;
        }
        [HttpGet]
        public async Task<IEnumerable<CustomerListDto>> GetList()
        {
            var customers = _customerRepository.
                        GetAllAsync();
            IEnumerable<CustomerListDto>? customerListDto = _mapper.Map<IEnumerable<CustomerListDto>>(customers);
            return customerListDto;
        }
        [HttpGet("{id}")]
        public CustomerDto Get(Guid id)
        {
            var customer = _customerRepository.GetByIdAsync(id);

            CustomerDto customerDto = _mapper.Map<CustomerDto>(customer);
            return customerDto;
        }
        [HttpPost]
        public CreateCustomerDto Create(CreateCustomerDto createCustomerDto)
        {
            Customer toCreateCustomer = _mapper.Map<Customer>(createCustomerDto);
            _customerRepository.AddAsync(toCreateCustomer);
            return createCustomerDto;
        }

        [HttpPut("{id}")]
        public UpdateCustomerDto Update(Guid id, UpdateCustomerDto updateCustomerDto)
        {
            
            var customer = _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return null;
            }
            else
            {

                var newCustomer = _mapper.Map<Customer>(updateCustomerDto);
                _customerRepository.UpdateAsync(id, newCustomer);
                return updateCustomerDto;
            }


        }
        [HttpDelete("{id}")]
        public CustomerDto Delete(Guid id)
        {
            var customer = _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return null;
            }
            _customerRepository.DeleteAsync(id);

            CustomerDto customerDto = _mapper.Map<CustomerDto>(customer);

            return customerDto;
        }
    }
}
