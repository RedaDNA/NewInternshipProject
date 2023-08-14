using APIPart.DTOs.CarDtos;

namespace APIPart.DTOs.RentalDtos
{
    public class RentalPaginationDto
    {
        public int Count { get; set; }
        public List<RentalListDto> RentalList { get; set; }
    }
}
