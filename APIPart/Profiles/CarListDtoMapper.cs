using APIPart.DTOs;
using AutoMapper;
using Core.Entities;
using System.Linq;

namespace APIPart.Profiles
{
    public class CarListDtoMapper : ITypeConverter< List<CarListDto>, CarPaginationDto>
    {
        

        public CarPaginationDto Convert(List<CarListDto> source, CarPaginationDto destination, ResolutionContext context)
        {
            return context.Mapper.Map<CarPaginationDto>(source);
        }
    }
}
