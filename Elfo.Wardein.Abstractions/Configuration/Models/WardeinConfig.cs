using Elfo.Wardein.Abstractions.Configuration.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Abstractions.Configuration.Models
{
    public class WardeinConfig
    {
        [JsonProperty(PropertyName = "timeSpanFromSeconds")]
        public double TimeSpanFromSeconds { get; set; }

        [JsonProperty(PropertyName = "maintenanceModeStatus")]
        public MaintenanceModeStatus MaintenanceModeStatus { get; set; } = new MaintenanceModeStatus()
        {
            DurationInSeconds = 300,
            IsInMaintenanceMode = false,
            MaintenanceModeStartDateInUTC = DateTime.UtcNow.AddMinutes(-1)
        }; // Default values

        [JsonProperty(PropertyName = "heartbeat")]
        public HeartbeatConfigurationModel Heartbeat { get; set; } = new HeartbeatConfigurationModel();

        [JsonProperty(PropertyName = "services")]
        public IEnumerable<GenericServiceConfigurationModel> Services { get; set; }

        [JsonProperty(PropertyName = "iisPools")]
        public IEnumerable<GenericServiceConfigurationModel> IISPools { get; set; }

        [JsonProperty(PropertyName = "urls")]
        public IEnumerable<WebWatcherConfigurationModel> Urls { get; set; }

        [JsonProperty(PropertyName = "cleanUps")]
        public IEnumerable<FileSystemConfigurationModel> CleanUps { get; set; }
    }
}
