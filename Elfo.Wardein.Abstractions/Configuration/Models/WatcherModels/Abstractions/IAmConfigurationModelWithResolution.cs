using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Abstractions.Configuration.Models
{
    public interface IAmConfigurationModelWithResolution : IAmBaseConfigurationModel
    {
        [JsonProperty(PropertyName = "notificationtype")]
        public NotificationType NotificationType { get; set;  }

        [JsonProperty(PropertyName = "recipientAddresses")]
        public string RecipientAddresses { get; set; }

        [JsonProperty(PropertyName = "failureMessage")]
        public string FailureMessage { get; set; }

        [JsonProperty(PropertyName = "restoredMessage")]
        public string RestoredMessage { get; set; }

        [JsonProperty(PropertyName = "maxRetryCount")]
        public int MaxRetryCount { get; set; }

        [JsonProperty(PropertyName = "sendReminderEmailAfterRetryCount")]
        public int SendReminderEmailAfterRetryCount { get; set; }

        [JsonProperty(PropertyName = "sendSuccessMailOnlyIfMaxRetryCountExceeded")]
        public bool SendSuccessMailOnlyIfMaxRetryCountExceeded { get; set; }
    }
}
