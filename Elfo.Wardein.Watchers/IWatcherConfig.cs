using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Watchers
{
    public interface IWatcherConfig
    {
        bool IsInMaintenanceMode { get; }

        double TimeSpanFromSeconds { get; }
    }
}
