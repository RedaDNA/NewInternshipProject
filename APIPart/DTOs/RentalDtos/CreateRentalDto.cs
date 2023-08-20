using Core.enums;
using System.ComponentModel.DataAnnotations;

namespace APIPart.DTOs.RentalDtos
{
    public class CreateRentalDto
    {
        [DataType(DataType.DateTime, ErrorMessage = "The StartDate field must be a valid DateTime value.")]
        [Required(ErrorMessage = "The StartDate field is required.")]

        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "The EndDate field is required.")]
        [DataType(DataType.DateTime, ErrorMessage = "The EndDate field must be a valid DateTime value.")]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "The CarId field is required.")]
        public Guid CarId { get; set; }
        [Required(ErrorMessage = "The CustomerId field is required.")]
        public Guid CustomerId { get; set; }
        public Guid? DriverId { get; set; }
        [Required(ErrorMessage = "The TotalFare field is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "The TotalFare field must be a non-negative value.")]
        public decimal TotalFare { get; set; }
        [Required(ErrorMessage = "The Status field is required.")]
        public RentalStatus? Status { get; set; }
    }
}
