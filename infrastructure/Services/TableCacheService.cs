using Core.Entities;
using Core.Interfaces.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Services
{
    public class TableCacheService : ITableCacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ICarService _carService;

        public TableCacheService(IMemoryCache memoryCache, ICarService carService)
        {
            _memoryCache = memoryCache;
            _carService = carService;
        }

        public async Task CacheData()
        {

            var carList = await _carService.GetQueryable().Include(r => r.Driver).ToListAsync(); 
            // Retrieve the data from the database and cache it
          //  var carList = await _carService.GetQueryable().Include(r => r.Driver).ToListAsync();

            var cacheExpirationOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(1)
            };

            _memoryCache.Set("CarList", carList, cacheExpirationOptions);
        }
    }
}
