﻿using Core.enums;

namespace APIPart.DTOs.RentalDtos
{
    public class UpdateRentalDto
    {

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid CarId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? DriverId { get; set; }

        public decimal TotalFare { get; set; }
        public RentalStatus Status { get; set; }
    }
}
