using APIPart.DTOs.RentalDtos;
using AutoMapper;
using Core.Entities;

namespace APIPart.Profiles
{
    public class RentalProfile : Profile
    {
        public RentalProfile()
        {
            CreateMap<Rental, RentalDto>();
            CreateMap<CreateRentalDto, Rental>();

            CreateMap<UpdateRentalDto, Rental>();
            CreateMap<RentalListDto, Rental>();
            CreateMap<Rental, RentalListDto>();


            CreateMap<List<Rental>, RentalPaginationDto>().ForMember(des => des.RentalList,
                src => src.MapFrom(c => c));




        }
    }
}