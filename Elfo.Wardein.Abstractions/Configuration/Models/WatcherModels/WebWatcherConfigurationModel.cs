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
        
        public int WatcherConfigurationId { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationHostname => HostHelper.GetName();
        public NotificationType NotificationType { get; set; } = NotificationType.Mail;
        public string RecipientAddresses { get; set; }
        public string FailureMessage { get; set; }
        public string RestoredMessage { get; set; }
        public int MaxRetryCount { get; set; } = 2;
        public int SendReminderEmailAfterRetryCount { get; set; } = 120;

        public HttpCallApiMethod Method { get; set; } = HttpCallApiMethod.Get;

        /// <summary>
        /// Request Body that may be required for POST request.
        /// </summary>
        public object Body { get; set; } = null;

        /// <summary>
        /// Request headers.
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }
    }

    public enum HttpCallApiMethod
    {
        Get = 1,
        Post = 2
    }
}
