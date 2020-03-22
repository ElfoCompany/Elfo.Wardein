using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Elfo.Wardein.Abstractions.Configuration.Models
{
    public class HeartbeatConfigurationModel : IAmBaseConfigurationModel
    {
        public string ApplicationHostname => HostHelper.GetName();

        public bool IsInMaintenanceMode => false;

        public double TimeSpanFromSeconds => 60;

        public int WatcherConfigurationId { get; set; }

        public int ApplicationId { get; set; }
    }
}
