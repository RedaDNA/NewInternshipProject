using System.ComponentModel.DataAnnotations;

namespace APIPart.DTOs.UserDtos
{
    public class AuthUserDto
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username must be between 1 and 50 characters", MinimumLength = 1)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "Password must be between 6 and 100 characters", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
