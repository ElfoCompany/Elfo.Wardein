using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace Elfo.Wardein.Watchers.HeartBeat.Config
{
    public class HeartBeatWatcherConfig : IWatcherConfig
    {
        /// <summary>
        /// Property defines if watcher has to be running in maintainance mode
        /// Default value false
        /// </summary>
        [JsonProperty(PropertyName = "isInMaintenanceMode")]
        public bool IsInMaintenanceMode { get; set; } = false;

        [JsonProperty(PropertyName = "heartBeatAppName")]
        public string HeartBeatAppName { get; set; } = Dns.GetHostName()?.ToUpperInvariant();

        /// <summary>
        /// Property that defines frequency of watcher polling
        /// Default value 10 seconds
        /// </summary>
        [JsonProperty(PropertyName = "timeSpanFromSeconds")]
        public double TimeSpanFromSeconds { get; set; } = 10;

        [JsonProperty(PropertyName = "sendRepeatedNotificationAfterSeconds")]
        public double SendRepeatedNotificationAfterSeconds { get; set; } = 3600; // Default values

        [JsonProperty(PropertyName = "numberOfNotificationsWithoutRateLimitation")]
        public int NumberOfNotificationsWithoutRateLimitation { get; set; } = 2; // Default values

        [JsonProperty(PropertyName = "connectionString")]
        public string ConnectionString { get; set; }

    }
}
