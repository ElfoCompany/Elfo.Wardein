using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Elfo.Wardein.Abstractions.Configuration.Models.WatcherModels
{
	public class BaseUrlWatcherConfigurationModel : IAmConfigurationModelWithResolution
	{
		[JsonProperty(PropertyName = "url")]
		public Uri Url { get; set; }
		[JsonProperty(PropertyName = "urlAlias")]
		public string UrlAlias { get; set; }
		[JsonProperty(PropertyName = "headers")]
		public IDictionary<string, string> Headers { get; set; }
        [JsonProperty(PropertyName = "associatedIISPool")]
        public string AssociatedIISPool { get; set; }

        #region IAmConfigurationModelWithResolution
        [JsonProperty(PropertyName = "notificationtype")]
        public NotificationType NotificationType { get; set; } = NotificationType.Mail;
        [JsonProperty(PropertyName = "recipientAddresses")]
        public string RecipientAddresses { get; set; }
        [JsonProperty(PropertyName = "failureMessage")]
        public string FailureMessage { get; set; }
        [JsonProperty(PropertyName = "restoredMessage")]
        public string RestoredMessage { get; set; }
        [JsonProperty(PropertyName = "maxRetryCount")]
        public int MaxRetryCount { get; set; } = 2;
        [JsonProperty(PropertyName = "sendReminderEmailAfterRetryCount")]
        public int SendReminderEmailAfterRetryCount { get; set; } = 120;
        [JsonProperty(PropertyName = "sendSuccessMailOnlyIfMaxRetryCountExceeded")]
        public bool SendSuccessMailOnlyIfMaxRetryCountExceeded { get; set; } = false;
        #endregion

        #region IAmBaseConfigurationModel
        [JsonProperty(PropertyName = "timeSpanFromSeconds")]
        public double? TimeSpanFromSeconds { get; set;  }
        [JsonProperty(PropertyName = "watcherConfigurationId")]
        public int WatcherConfigurationId { get; set; }
        [JsonProperty(PropertyName = "applicationId")]
        public int ApplicationId { get; set; }
        [JsonProperty(PropertyName = "applicationHostname")]
        public string ApplicationHostname { get; set; } = HostHelper.GetName();
        #endregion
    }
}
