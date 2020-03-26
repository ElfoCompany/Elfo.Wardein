using Elfo.Wardein.Abstractions.Configuration.Models;
using System.Threading.Tasks;
using static Elfo.Wardein.Abstractions.Configuration.Models.WebWatcherConfigurationModel;

namespace Elfo.Wardein.Abstractions.WebWatcher
{
    public interface IAmUrlResponseManager
    {
        Task<bool> IsHealthy(WebWatcherConfigurationModel configuration);
        Task RestartPool(string poolName);
    }
}
