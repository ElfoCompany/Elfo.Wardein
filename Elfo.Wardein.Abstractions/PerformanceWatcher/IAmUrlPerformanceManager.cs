using Elfo.Wardein.Abstractions.BaseUrlWatcher;
using Elfo.Wardein.Abstractions.Configuration.Models;

namespace Elfo.Wardein.Abstractions.PerformanceWatcher
{
    public interface IAmUrlPerformanceManager : IAmUrlManager<PerformanceWatcherConfigurationModel> { }
}
