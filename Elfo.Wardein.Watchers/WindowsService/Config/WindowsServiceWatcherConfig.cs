using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Watchers.WindowsService
{
    public class WindowsServiceWatcherConfig : IWatcherConfig
    {
        /// <summary>
        /// Property defines if watcher has to be running in maintainance mode
        /// Default value false
        /// </summary>
        [JsonProperty(PropertyName = "isInMaintenanceMode")]
        public bool IsInMaintenanceMode { get; set; } = false;

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

        [JsonProperty(PropertyName = "services")]
        public IEnumerable<ObservableWindowsService> Services { get; set; }
    }
}
