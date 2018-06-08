using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Elfo.Wardein.Core.Abstractions
{
    public interface IAmNotificationService
    {
        Task SendNotificationAsync(string recipientAddress, string notificationBody, string notificationTitle);
    }
}
