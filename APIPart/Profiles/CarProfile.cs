using APIPart.DTOs;
using Core.Entities;
using AutoMapper;
namespace APIPart.Profiles
{
    public class CarProfile : Profile
    {
        public  CarProfile() { CreateMap<Car, CarDTO>(); }
    }
}
