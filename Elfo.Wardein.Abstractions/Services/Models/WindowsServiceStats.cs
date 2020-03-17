using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Abstractions.Services.Models
{
    public class WindowsServiceStats
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "retryCount")]
        public int RetryCount { get; set; }

        [JsonProperty(PropertyName = "lastNotificationSentAtThisTimeUTC")]
        public DateTime? LastNotificationSentAtThisTimeUTC { get; set; }
    }
}
