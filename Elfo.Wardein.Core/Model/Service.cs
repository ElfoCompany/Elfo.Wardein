﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Core.Model
{
    public class Service
    {
        [JsonProperty(PropertyName = "serviceName")]
        public string ServiceName { get; set; }
        [JsonProperty(PropertyName = "maxRetryCount")]
        public int MaxRetryCount { get; set; }
        [JsonProperty(PropertyName = "failMessage")]
        public string FailMessage { get; set; }
        [JsonProperty(PropertyName = "restoredMessage")]
        public string RestoredMessage { get; set; }
        [JsonProperty(PropertyName = "recipientAddress")]
        public string RecipientAddress { get; set; }
        [JsonProperty(PropertyName = "notificationType")]
        public string NotificationType { get; set; } = "Mail";
    }
}
