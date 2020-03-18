namespace Elfo.Wardein.Watchers
{
    public interface IWatcherConfig
    {
        bool IsInMaintenanceMode { get; }

        double TimeSpanFromSeconds { get; }
    }
}
