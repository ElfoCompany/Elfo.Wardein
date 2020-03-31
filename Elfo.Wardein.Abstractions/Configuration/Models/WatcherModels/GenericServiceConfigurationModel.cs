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
        [JsonProperty(PropertyName = "timeSpanFromSeconds")]
        public double TimeSpanFromSeconds { get; set; } = 60;
        [JsonProperty(PropertyName = "watcherConfigurationId")]
        public int WatcherConfigurationId { get; set; }
        [JsonProperty(PropertyName = "applicationId")]
        public int ApplicationId { get; set; }
        [JsonProperty(PropertyName = "applicationHostname")]
        public string ApplicationHostname { get; set; } = HostHelper.GetName();
        [JsonProperty(PropertyName = "recipientAddresses")]
        public string RecipientAddresses { get; set; }
        [JsonProperty(PropertyName = "failureMessage")]
        public string FailureMessage { get; set; }
        [JsonProperty(PropertyName = "sendReminderEmailAfterRetryCount")]
        public int SendReminderEmailAfterRetryCount { get; set; } = 120;
        [JsonProperty(PropertyName = "notificationType")]
        public NotificationType NotificationType { get; set; } = NotificationType.Mail;
        [JsonProperty(PropertyName = "restoredMessage")]
        public string RestoredMessage { get; set; }
        [JsonProperty(PropertyName = "maxRetryCount")]
        public int MaxRetryCount { get; set; } = 2;
        [JsonProperty(PropertyName = "sendSuccessMailOnlyIfMaxRetryCountExceeded")]
        public bool SendSuccessMailOnlyIfMaxRetryCountExceeded { get; set; } = false;
    }
}
