using Core.enums;   

namespace APIPart.DTOs.CarDtos
{
    public class CarRequestDto
    {
      public int PageSize { get; set; }
        public int PageNumber { get; set; } = 1;

        public string? SearchWord { get; set; } = "";
        public SortingType? SortingType { get; set; }
    }
}
