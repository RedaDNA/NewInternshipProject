using Core.enums;

namespace APIPart.DTOs.RentalDtos
{
    public class RentalListDto
    {
        public Guid Id { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid CarId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? DriverId { get; set; }

        public decimal TotalFare { get; set; }
        public RentalStatus Status { get; set; }
        public string CarColor { get; set; }
        public string CarType { get; set; }
        public string CustomerName { get; set; }
        public string DriverName { get; set; }
        public string CarNumber { get; internal set; }
    }
}
