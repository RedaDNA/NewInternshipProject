using Core.Entities;

namespace APIPart.DTOs.DriverDtos
{
    public class DriverDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string LicenseNumber { get; set; }
        public Guid? ReplacementDriverId { get; set; }
        public bool HasReplacement { get; set; }

        public bool IsAvailable { get; set; }
    }
}
