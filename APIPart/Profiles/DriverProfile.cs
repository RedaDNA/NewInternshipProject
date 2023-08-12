using APIPart.DTOs;
using APIPart.DTOs.CarDtos;
using APIPart.DTOs.DriverDtos;
using AutoMapper;
using Core.Entities;

namespace APIPart.Profiles
{
    public class DriverProfile : Profile
    {
        public DriverProfile()
        {
            CreateMap<Driver, DriverDto>();

            CreateMap<CreateDriverDto, Driver>();

            CreateMap<UpdateDriverDto, Driver>();
            CreateMap<DriverListDto, Driver>();
            CreateMap<Driver, DriverListDto>();
          

            CreateMap<List<Driver>, DriverPaginationDto>().ForMember(des => des.DriverList,
             src => src.MapFrom(c => c));
        }

    }
}
