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
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly CarRentalContext _context;
        public CustomerRepository(CarRentalContext context) : base(context)
        {
            _context = context;

        }
    
    }
}
