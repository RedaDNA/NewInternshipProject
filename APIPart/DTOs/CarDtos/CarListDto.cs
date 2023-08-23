namespace APIPart.DTOs.CarDtos
{
    public class CarListDto
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public string Type { get; set; }
        public int EngineCapacity { get; set; }
        public string Color { get; set; }
        public decimal DailyFare { get; set; }
        public Guid? DriverId { get; set; }
        public bool IsAvailable { get; set; }
        public string DriverName { get; set; }


    }
}
