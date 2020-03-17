using Elfo.Wardein.Abstractions.Configuration;
using Newtonsoft.Json;

namespace Elfo.Wardein.Abstractions.Configuration.Models
{
    public class MailConfiguration
    {
        [JsonProperty(PropertyName = "host")]
        public string Host { get; set; }
        [JsonProperty(PropertyName = "port")]
        public int Port { get; set; }
        [JsonProperty(PropertyName = "enableSSL")]
        public bool EnableSSL { get; set; }
        [JsonProperty(PropertyName = "deliveryMethod")]
        public string DeliveryMethod { get; set; }
        [JsonProperty(PropertyName = "useDefaultCredentials")]
        public bool UseDefaultCredentials { get; set; }
        [JsonProperty(PropertyName = "fromAddress")]
        public string FromAddress { get; set; }
        [JsonProperty(PropertyName = "fromDisplayName")]
        public string FromDisplayName { get; set; }
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }
        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }
    }
}
