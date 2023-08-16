using APIPart.DTOs.RentalDtos;
using APIPart.DTOs.UserDtos;
using Core.Entities;
using AutoMapper;


namespace APIPart.Profiles
{
    public class UserProfile :Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<CreateUserDto, User>();

            CreateMap<UpdateUserDto, User>();
            CreateMap<UserListDto, User>();
            CreateMap<User, UserListDto>();


            CreateMap<List<User>, UserPaginationDto>().ForMember(des => des.UserList,
                src => src.MapFrom(c => c));




        }
    }
}
