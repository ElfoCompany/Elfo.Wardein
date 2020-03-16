using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Watchers.FileSystem
{
    public class FileSystemWatcherConfig : IWatcherConfig
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

        /// <summary>
        /// List of folder configurations that has to be monitored and cleaned by criteria
        /// </summary>
        [JsonProperty(PropertyName = "cleanUps")]
        public IEnumerable<FileSystemCleanUpConfig> CleanUps { get; set; } = new List<FileSystemCleanUpConfig>();
    }
}
