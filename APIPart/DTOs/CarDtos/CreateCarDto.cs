using System.ComponentModel.DataAnnotations;

namespace APIPart.DTOs.CarDtos
{
    public class CreateCarDto
    {

        [Required(ErrorMessage = "Number is required")]
        public string Number { get; set; }
        [Required(ErrorMessage = "Type is required")]
        public string Type { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "Engine capacity must be a positive value")]
        public int EngineCapacity { get; set; }
        [Required(ErrorMessage = "Color is required")]
        public string Color { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Daily fare must be a positive value")]
        public decimal DailyFare { get; set; }
        public bool IsAvailable { get; set; }  = true;
        public Guid? DriverId { get; set; }
    }
}



