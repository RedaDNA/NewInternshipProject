using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Data
{
    internal class DriverRepository : GenericRepository<Driver>, IDriverRepository
    {
        private readonly CarRentalContext _context;
        public DriverRepository(CarRentalContext context) : base(context)
        {
            _context = context;

        }
       

    }
   
}
