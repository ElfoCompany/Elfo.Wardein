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
    }
}
