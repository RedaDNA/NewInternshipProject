using APIPart.DTOs;
using Core.Entities;
using AutoMapper;
namespace APIPart.Profiles
{
    public class CarProfile : Profile
    {
        public CarProfile()
        {
            CreateMap<CarDTO, Car>();
            CreateMap<CarPaginationDto, List<CarListDto>>();
           CreateMap<List<CarListDto>, CarPaginationDto>()
            .ForMember(d => d.CarList, 
            opt => opt.MapFrom(src => src))
                    .ForMember(d => d.Count,
                    
                    opt => opt.MapFrom(src => src.Count()));
        }


    }

}
