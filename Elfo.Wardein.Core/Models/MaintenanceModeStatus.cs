using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Core.Models
{
    public class MaintenanceModeStatus
    {
        [JsonProperty(PropertyName = "maintenanceModeStartDateInUTC")]
        public DateTime MaintenanceModeStartDateInUTC { get; set; } = DateTime.UtcNow;
        [JsonProperty(PropertyName = "isInMaintenanceMode")]
        public bool IsInMaintenanceMode { get; set; }
        [JsonProperty(PropertyName = "durationInSeconds")]
        public double DurationInSeconds { get; set; } = 300;
    }
}
