using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Abstractions.Configuration.Models
{
    public interface IAmBaseConfigurationModel
    {
        [JsonProperty(PropertyName = "timeSpanFromSeconds")]
        double TimeSpanFromSeconds { get; }
        [JsonProperty(PropertyName = "watcherConfigurationId")]
        public int WatcherConfigurationId { get; }
        [JsonProperty(PropertyName = "applicationId")]
        public int ApplicationId { get; }
        [JsonProperty(PropertyName = "applicationHostname")]
        public string ApplicationHostname { get; }
    }
}
