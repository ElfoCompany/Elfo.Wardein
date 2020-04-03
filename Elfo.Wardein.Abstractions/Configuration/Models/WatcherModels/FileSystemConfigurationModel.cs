using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Abstractions.Configuration.Models
{
    public class FileSystemConfigurationModel : IAmBaseConfigurationModel
    {
        /// <summary>
        /// Property that defines frequency of watcher polling
        /// Default value 10 seconds
        /// </summary>
        [JsonProperty(PropertyName = "timeSpanFromSeconds")]
        public double? TimeSpanFromSeconds { get; set; }

        /// <summary>
        /// List of folder configurations that has to be monitored and cleaned by criteria
        /// </summary>
        [JsonProperty(PropertyName = "cleanUps")]
        public IEnumerable<FileSystemCleanUpConfig> CleanUps { get; set; } = new List<FileSystemCleanUpConfig>();
        [JsonProperty(PropertyName = "watcherConfigurationId")]
        public int WatcherConfigurationId { get; set; }
        [JsonProperty(PropertyName = "applicationId")]
        public int ApplicationId { get; set; }
        [JsonProperty(PropertyName = "applicationHostname")]
        public string ApplicationHostname => HostHelper.GetName();
    }

    public class FileSystemCleanUpConfig
    {
        [JsonProperty(PropertyName = "filePath")]
        public string FilePath { get; set; }

        [JsonProperty(PropertyName = "timeSpanFromSeconds")]
        public double TimeSpanFromSeconds { get; set; } = 10; // Default values

        [JsonProperty(PropertyName = "cleanUpOptions")]
        public FileSystemCleanUpOptions CleanUpOptions { get; set; } = new FileSystemCleanUpOptions();
    }

    public class FileSystemCleanUpOptions
    {
        [JsonProperty(PropertyName = "thresholdInSeconds")]
        public int ThresholdInSeconds { get; set; }

        [JsonProperty(PropertyName = "thresholdInDays")]
        public int ThresholdInDays { get; set; }

        [JsonProperty(PropertyName = "displayOnly")]
        public bool DisplayOnly { get; set; } = false;

        [JsonProperty(PropertyName = "useRecycleBin")]
        public bool UseRecycleBin { get; set; } = false;

        [JsonProperty(PropertyName = "removeEmptyFolders")]
        public bool RemoveEmptyFolders { get; set; } = false;

        [JsonProperty(PropertyName = "recursive")]
        public bool Recursive { get; set; } = false;
    }
}
