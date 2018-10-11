using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Core.Models
{
    public class CleanUpParams
    {

        [JsonProperty(PropertyName = "thresholdInSeconds")]
        public int ThresholdInSeconds { get; set; } = 300;

        [JsonProperty(PropertyName = "thresholdInDays")]
        public int ThresholdInDays { get; set; } = 5;

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
