using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Core.Models
{
    public class CleanUps
    {
        [JsonProperty(PropertyName = "filePath")]
        public string FilePath { get; set; }

        [JsonProperty(PropertyName = "timeSpanFromSeconds")]
        public double TimeSpanFromSeconds { get; set; } = 10; // Default values

        [JsonProperty(PropertyName = "cleanUpOptions")]
        public CleanUpParams CleanUpOptions { get; set; } = new CleanUpParams();
    }
}
