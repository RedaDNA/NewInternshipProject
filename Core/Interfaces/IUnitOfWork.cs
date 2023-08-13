using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public  interface IUnitOfWork : IDisposable
    {
        ICarRepository Cars { get; }
       ICustomerRepository Customers { get; }
        IDriverRepository Drivers { get; }
        int Save();
    }
}
