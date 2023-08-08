using APIPart.DTOs.CarDtos;

namespace APIPart.DTOs.DriverDtos
{
    public class DriverPaginationDto
    {
        public int Count { get; set; }
        public List<DriverListDto> DriverList{ get; set; }
    }
}
