using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Watchers.FileSystem
{
    public class FileSystemCleanUpConfig
    {
        [JsonProperty(PropertyName = "filePath")]
        public string FilePath { get; set; }

        [JsonProperty(PropertyName = "timeSpanFromSeconds")]
        public double TimeSpanFromSeconds { get; set; } = 10; // Default values

        [JsonProperty(PropertyName = "cleanUpOptions")]
        public FileSystemCleanUpOptions CleanUpOptions { get; set; } = new FileSystemCleanUpOptions();
    }
}
