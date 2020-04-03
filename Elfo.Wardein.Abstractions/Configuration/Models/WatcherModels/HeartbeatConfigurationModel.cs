using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Elfo.Wardein.Abstractions.Configuration.Models
{
    public class HeartbeatConfigurationModel : IAmBaseConfigurationModel
    {
        [JsonProperty(PropertyName = "applicationHostname")]
        public string ApplicationHostname => HostHelper.GetName();
        [JsonProperty(PropertyName = "timeSpanFromSeconds")]
        public double? TimeSpanFromSeconds => 60;
        [JsonProperty(PropertyName = "watcherConfigurationId")]
        public int WatcherConfigurationId { get; set; }
        [JsonProperty(PropertyName = "applicationId")]
        public int ApplicationId { get; set; }
    }
}
