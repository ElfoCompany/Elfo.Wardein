using Elfo.Wardein.Abstractions.Configuration.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Elfo.Wardein.Abstractions.PerformanceWatcher
{
     public interface IAmUrlPerformanceManager
    {
        Task<bool> IsHealthy(PerformanceWatcherConfigurationModel configuration);
        Task RestartPool(string poolName);
    }
}
