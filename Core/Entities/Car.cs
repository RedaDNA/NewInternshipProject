using System;

namespace Core.Entities
{
    public class Car
    {
        public Guid Id { get; set; }
        public string Number { get; set; }
        public string Type { get; set; }
        public int EngineCapacity { get; set; }
        public string Color { get; set; }
        public decimal DailyFare { get; set; }
        public bool HasDriver { get; set; }
        public bool IsAvailable { get; set; }
        public Guid? DriverId { get; set; }

      
        public virtual ICollection<Rental> Rentals { get; set; }
    
        public virtual Driver Driver { get; set; }

    }

}
