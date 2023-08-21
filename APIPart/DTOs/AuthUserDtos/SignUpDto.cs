using System.ComponentModel.DataAnnotations;

namespace APIPart.DTOs.AuthUserDtos
{
    public class SignUpDto
    {
        [Required(ErrorMessage = "The UserName field is required.")]
        public  string UserName { get; set; }

        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "The Email field is not a valid email address.")]
        public  string Email { get; set; }

        [Required(ErrorMessage = "The PhoneNumber field is required.")]
        [Phone(ErrorMessage = "The PhoneNumber field is not a valid phone number.")]
        public  string PhoneNumber { get; set; }
        [Required(ErrorMessage = "The Password field is required.")]
        [MinLength(6, ErrorMessage = "The Password field must be at least 6 characters long.")]
        public string Password { get; set; }
        // Add any additional properties or methods as needed
    }
}
