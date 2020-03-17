using System.Threading.Tasks;

namespace Elfo.Wardein.Abstractions.Services
{
    public interface IAmNotificationService
    {
        Task SendNotificationAsync(string recipientAddress, string notificationBody, string notificationTitle);
    }
}
