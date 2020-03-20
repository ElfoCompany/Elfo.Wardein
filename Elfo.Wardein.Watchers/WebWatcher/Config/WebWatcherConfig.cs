using Newtonsoft.Json;

namespace Elfo.Wardein.Watchers.WebWatcher
{
    public class WebWatcherConfig : IWatcherConfig
    {
        [JsonProperty(PropertyName = "isInMaintenanceMode")]
        public bool IsInMaintenanceMode { get; protected set; } = false;

        [JsonProperty(PropertyName = "timeSpanFromSeconds")]
        public double TimeSpanFromSeconds { get; protected set; } = 10;

        [JsonProperty(PropertyName = "useAuthentication")]
        public bool UseAuthentication { get; protected set; }

        [JsonProperty(PropertyName = "authCredentials")]
        public string AuthCredentials { get; protected set; }

        [JsonProperty(PropertyName = "assertWithRegex")]
        public string AssertWithRegex { get; protected set; }

        [JsonProperty(PropertyName = "assertWithStatusCode")]
        public bool AssertWithStatusCode { get; protected set; }

        [JsonProperty(PropertyName = "maxRetryCount")]
        public int MaxRetryCount { get; protected set; }

        [JsonProperty(PropertyName = "sendReminderEmailAfterRetryCount")]
        public int SendReminderEmailAfterRetryCount { get; protected set; }

        [JsonProperty(PropertyName = "associatedIISPool")]
        public string AssociatedIISPool { get; protected set; }

        [JsonProperty(PropertyName = "recipientAddresses")]
        public string RecipientAddresses { get; protected set; }

        [JsonProperty(PropertyName = "failureMessage")]
        public string FailureMessage { get; protected set; }

        [JsonProperty(PropertyName = "restoredMessage")]
        public string RestoredMessage { get; protected set; }

        [JsonProperty(PropertyName = "connectionString")]
        public string ConnectionString { get; protected set; }
    }
}
