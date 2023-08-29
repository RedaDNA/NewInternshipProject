namespace APIPart.DTOs.CustomerDtos
{
    public class CustomerRequestDto
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; } = 1;

        public string? SearchWord { get; set; } = "";
        public string? SortingType { get; set; } 
        public string? SortingColumn { get; set; } ="";
    }
}
