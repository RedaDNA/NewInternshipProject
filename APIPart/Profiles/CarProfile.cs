using Core.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Routing.Constraints;
using APIPart.DTOs.CarDtos;

namespace APIPart.Profiles
{
    public class CarProfile : Profile
    {
        public CarProfile()
        {
            CreateMap<Car, CarDTO>();
            CreateMap <CreateCarDto, Car>();

             CreateMap<UpdateCarDto, Car>();
            CreateMap<CarListDto, Car>();
            CreateMap<Car, CarListDto>();
       

            CreateMap<List<Car>, CarPaginationDto>().ForMember(des => des.CarList,
                src => src.MapFrom(c => c));




            /* var configuration = new MapperConfiguration(cfg => {
                 cfg.CreateMap<CarListDto, Car>().ConvertUsing(s =>s.Id,d =>d.MapFrom(c => c.));

                 CreateMap<Car, CarPaginationDto>().ForMember(des => des.CarList,

                 opt => opt.MapFrom ( opt=>opt));  */
            //   CreateMap<Car, CarPaginationDto>().ForAllMembers(c => c.ConvertUsing(CarListDtoMapper)

            //  .ForMember(des => des.Count, opt => opt.MapFrom(opt =>opt.Count()));
            // CreateMap<CarPaginationDto, List<CarListDto>>();
            /*CreateMap<List<CarListDto>, CarPaginationDto>()
             .ForMember(d => d.CarList, 
             opt => opt.MapFrom(src => src))
                     .ForMember(d => d.Count,

                     opt => opt.MapFrom(src => src.Count()));*/
        }


    }

}
