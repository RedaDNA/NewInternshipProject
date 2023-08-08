using APIPart.DTOs;
using APIPart.DTOs.CustomerDtos;
using AutoMapper;
using Core.Entities;

namespace APIPart.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile() {
            CreateMap<Customer, CustomerDto>();

            CreateMap<CreateCustomerDto, Customer>();

            CreateMap<UpdateCustomerDto, Customer>();
            CreateMap<CustomerListDto, Customer>();
            CreateMap<Customer, CustomerListDto>();


            CreateMap<List<Customer>, CustomerPaginationDto>().ForMember(des => des.CustomerList,
             src => src.MapFrom(c => c));
        }
       
    }
}
