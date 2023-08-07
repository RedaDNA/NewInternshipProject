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
            CarPaginationDto carPaginationDto = new CarPaginationDto();
            carPaginationDto.CarList = source;
            carPaginationDto.Count = destination.Count;
            return carPaginationDto;
        }
    }
}
