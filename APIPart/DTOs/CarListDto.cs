namespace APIPart.DTOs
{
    public class CarListDto
    {
        public string Number { get; set; }
        public string Type { get; set; }
        public int EngineCapacity { get; set; }
        public string Color { get; set; }
        public decimal DailyFare { get; set; }
        public bool WithDriver { get; set; }
        public Guid? DriverId { get; set; }


    }
}
