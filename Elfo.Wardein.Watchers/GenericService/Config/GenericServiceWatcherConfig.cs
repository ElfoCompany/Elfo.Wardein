using Newtonsoft.Json;
using System.Collections.Generic;


namespace Elfo.Wardein.Watchers.GenericService
{
    public class GenericServiceWatcherConfig : IWatcherConfig 
    {
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
        public IEnumerable<ObservarableGenericServiceWatcher> Services { get; set; }

        [JsonProperty(PropertyName = "isInMaintenanceMode")]
        public bool IsInMaintenanceMode { get; set; } = false;
    }
}
