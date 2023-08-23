namespace Core.Entities
{ 
    public class Driver : BaseEntity
    {

       
        public string Name { get; set; }
        public string Phone { get; set; }
        public string LicenseNumber { get; set; }
        public Guid? ReplacementDriverId { get; set; }
        public virtual Driver? ReplacementDriver { get; set; }
        public virtual ICollection<Car> Cars { get; set; }
        public virtual ICollection<Rental> Rentals { get; set; }

        public bool IsAvailable { get; set; }
    }
}
