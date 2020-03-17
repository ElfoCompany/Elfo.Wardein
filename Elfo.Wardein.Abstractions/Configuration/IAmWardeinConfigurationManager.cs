using Elfo.Wardein.Abstractions.Configuration.Models;

namespace Elfo.Wardein.Abstractions.Configuration
{
    public interface IAmWardeinConfigurationManager : IAmBaseConfigurationManager<WardeinConfig>
    {
        bool IsInMaintenanceMode { get; }

        void StartMaintenanceMode(double durationInSeconds);

        void StopMaintenaceMode();
    }
}
