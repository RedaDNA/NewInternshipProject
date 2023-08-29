using Core.Interfaces.IServices;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Services
{
    public class DataCachingService : IHostedService
    {
        private readonly ITableCacheService _tableCacheService;

        public DataCachingService(ITableCacheService tableCacheService)
        {
            _tableCacheService = tableCacheService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Cache the data at startup
            await _tableCacheService.CacheData();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Perform any cleanup or additional tasks if needed
            return Task.CompletedTask;
        }
    }
}
