using Elfo.Wardein.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Core.Abstractions
{
    public interface IAmWardeinConfigurationManager : IAmBaseConfigurationManager<WardeinConfig>
    {
        bool IsInMaintenanceMode { get; }

        void StartMaintenanceMode(double durationInSeconds);

        void StopMaintenaceMode();

    }
}
