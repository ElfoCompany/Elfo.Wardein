﻿using Elfo.Wardein.Abstractions.Configuration.Models.WatcherModels;
using Newtonsoft.Json;

namespace Elfo.Wardein.Abstractions.Configuration.Models
{
    public class PerformanceWatcherConfigurationModel : BaseUrlWatcherConfigurationModel
    {
        [JsonProperty(PropertyName = "assertWithRegex")]
        public string AssertWithRegex { get; set; }
        [JsonProperty(PropertyName = "assertWithStatusCode")]
        public bool AssertWithStatusCode { get; set; } = true;
        [JsonProperty(PropertyName = "method")]
        public HttpCallApiMethod Method { get; set; } = HttpCallApiMethod.Get;
        [JsonProperty(PropertyName = "body")]
        public object Body { get; set; } = null;
        [JsonProperty(PropertyName = "firstThresholdInMilliseconds")]
        public int FirstThresholdInMilliseconds { get; set; }
    }
}