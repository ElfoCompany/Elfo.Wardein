using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Abstractions.Configuration.Models
{
    public class WebWatcherConfigurationModel : IAmConfigurationModelWithResolution
    {
        [JsonProperty(PropertyName = "associatedIISPool")]
        public string AssociatedIISPool { get; set; }
        [JsonProperty(PropertyName = "url")]
        public Uri Url { get; set; }
        [JsonProperty(PropertyName = "urlAlias")]
        public string UrlAlias { get; set; }
        [JsonProperty(PropertyName = "assertWithRegex")]
        public string AssertWithRegex { get; set; }
        [JsonProperty(PropertyName = "assertWithStatusCode")]
        public bool AssertWithStatusCode { get; set; } = true;
        [JsonProperty(PropertyName = "timeSpanFromSeconds")]
        public double TimeSpanFromSeconds { get; set; } = 60;
        [JsonProperty(PropertyName = "watcherConfigurationId")]
        public int WatcherConfigurationId { get; set; }
        [JsonProperty(PropertyName = "applicationId")]
        public int ApplicationId { get; set; }
        [JsonProperty(PropertyName = "applicationHostname")]
        public string ApplicationHostname { get; set; } = HostHelper.GetName();
        [JsonProperty(PropertyName = "notificationType")]
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
        [JsonProperty(PropertyName = "method")]
        public HttpCallApiMethod Method { get; set; } = HttpCallApiMethod.Get;
        [JsonProperty(PropertyName = "body")]
        public object Body { get; set; } = null;
        [JsonProperty(PropertyName = "headers")]
        public IDictionary<string, string> Headers { get; set; }
        [JsonProperty(PropertyName = "sendSuccessMailOnlyIfMaxRetryCountExceeded")]
        public bool SendSuccessMailOnlyIfMaxRetryCountExceeded { get; set; } = false;
    }

    public enum HttpCallApiMethod
    {
        Get = 1,
        Post = 2
    }
}
