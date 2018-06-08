using Elfo.Wardein.Core.Abstractions;
using Elfo.Wardein.Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Elfo.Wardein.Core.NotificationService
{
    public class TeamsNotificationService : IAmNotificationService
    {
        public async Task SendNotificationAsync(string recipientAddress, string notificationBody, string notificationTitle)
        {
            var message = new MicrosoftTeamsMessage { Text = notificationBody, Title = notificationTitle };
            var jsonMessage = JsonConvert.SerializeObject(message, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var content = new StringContent(jsonMessage.ToString(), Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            await client.PostAsync(recipientAddress, content);
        }
    }
}
