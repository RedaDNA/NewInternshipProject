namespace APIPart.DTOs.RentalDtos
{
    public class RentalRequestDto
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; } = 1;

        public string? SearchWord { get; set; } = "";
        public string? SortingType { get; set; }
        public string? SortingColumn { get; set; } ="";
    }
}
