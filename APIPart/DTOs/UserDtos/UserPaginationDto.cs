using APIPart.DTOs.RentalDtos;

namespace APIPart.DTOs.UserDtos
{
    public class UserPaginationDto
    {
        public int Count { get; set; }
        public List<UserListDto> UserList { get; set; }
    }
}
