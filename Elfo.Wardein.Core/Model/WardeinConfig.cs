using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Core.Model
{
    public class WardeinConfig
    {
        [JsonProperty(PropertyName = "timeSpanFromSeconds")]
        public double TimeSpanFromSeconds { get; set; }
        [JsonProperty(PropertyName = "services")]
        public IList<WindowsService> Services { get; set; }
        [JsonProperty(PropertyName = "persistenceType")]
        public string PersistenceType { get; set; } = "JSON";
    }
}
