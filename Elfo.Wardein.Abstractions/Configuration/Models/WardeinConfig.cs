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

        [JsonProperty(PropertyName = "connectionString")]
        public string ConnectionString { get; set; }

        [JsonProperty(PropertyName = "sendRepeatedNotificationAfterSeconds")]
        public double SendRepeatedNotificationAfterSeconds { get; set; } = 3600; // Default values

        [JsonProperty(PropertyName = "numberOfNotificationsWithoutRateLimitation")]
        public int NumberOfNotificationsWithoutRateLimitation { get; set; } = 2; // Default values

        [JsonProperty(PropertyName = "persistenceType")]
        public string PersistenceType { get; set; } = "JSON";

        [JsonProperty(PropertyName = "maintenanceModeStatus")]
        public MaintenanceModeStatus MaintenanceModeStatus { get; set; } = new MaintenanceModeStatus()
        {
            DurationInSeconds = 300,
            IsInMaintenanceMode = false,
            MaintenanceModeStartDateInUTC = DateTime.UtcNow
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
