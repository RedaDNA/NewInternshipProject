using System.ComponentModel.DataAnnotations;

namespace APIPart.DTOs.CustomerDtos
{
    public class UpdateCustomerDto
    {
    
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone must be a 10-digit number")]
        public string Phone { get; set; }
    }
}
