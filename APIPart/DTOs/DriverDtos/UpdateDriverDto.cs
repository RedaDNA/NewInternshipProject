using System.ComponentModel.DataAnnotations;

namespace APIPart.DTOs.DriverDtos
{
    public class UpdateDriverDto
    {
      

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone must be a 10-digit number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "License number is required")]
        public string LicenseNumber { get; set; }

        public Guid? ReplacementDriverId { get; set; }

        public bool IsAvailable { get; set; }
    }
}
