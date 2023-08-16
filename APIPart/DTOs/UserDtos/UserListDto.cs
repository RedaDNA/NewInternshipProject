namespace APIPart.DTOs.UserDtos
{
    public class UserListDto
    {
        public Guid Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }
}
