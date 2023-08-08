namespace APIPart.DTOs.CustomerDtos
{
    public class CustomerPaginationDto
    {
        public int Count { get; set; }
        public List<CustomerListDto> CustomerList { get; set; }
    }
}
