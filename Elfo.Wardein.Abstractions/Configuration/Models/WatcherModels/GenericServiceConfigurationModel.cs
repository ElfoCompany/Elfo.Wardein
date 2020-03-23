using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Abstractions.Configuration.Models
{
    public class GenericServiceConfigurationModel : IAmConfigurationModelWithResolution
    {
        [JsonProperty(PropertyName = "serviceName")]
        public string ServiceName { get; set; }
        [JsonProperty(PropertyName = "serviceManagerType")]
        public ServiceManagerType ServiceManagerType { get; set; }
        public bool IsInMaintenanceMode { get; set; }
        public double TimeSpanFromSeconds { get; set; } = 60;
        public int WatcherConfigurationId { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationHostname => HostHelper.GetName();
        public string RecipientAddresses { get; set; } = "yaroslav.husynin@elfo.net;";
        public string FailureMessage { get; set; }
        public int SendReminderEmailAfterRetryCount { get; set; } = 120;
        public NotificationType NotificationType { get; set; } = NotificationType.Mail;
        public string RestoredMessage { get; set; } = "Restored Message Test";
        public int MaxRetryCount { get; set; } = 2;
    }
}
