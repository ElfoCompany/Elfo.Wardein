using Elfo.Wardein.Core.ConfigurationReader;
using Elfo.Wardein.Core.Helpers;
using Elfo.Wardein.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Elfo.Wardein.Core.Abstractions;
using Elfo.Wardein.Core.Persistence;
using Elfo.Wardein.Core.NotificationService;

namespace Elfo.Wardein.Core
{
    public class WardeinInstance
    {
        private string configPath = $@"{Const.BASE_PATH}Assets\WardeinConfig.json";
        private string dbPath = $@"{Const.BASE_PATH}Assets\WardeinDB.json";
        private WardeinConfig wardeinConfig = null;
        private readonly IAmPersistenceService persistenceService;

        public WardeinInstance()
        {
            if (!File.Exists(configPath))
            {
                //TODO: throw error or something...
            }
            else
            {
                this.wardeinConfig = GetWarderinConfigAndThrowErrorIfNotExist();
                this.persistenceService = new JSONPersistence(dbPath); // TODO: Add DI
            }

            WardeinConfig GetWarderinConfigAndThrowErrorIfNotExist()
            {
                var conf = new WardeinConfigurationReader(configPath).GetConfiguration();
                if (conf == null)
                    throw new ArgumentNullException("Wardein configuration not found or not well formatted");
                return conf;
            }
        }

        public async Task RunCheck()
        {
            foreach (var service in wardeinConfig.Services)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(250)); // TODO: Do we really need this?

                var serviceHelper = new WindowsServiceHelper(service.ServiceName);
                var notificationService = new MailNotificationService(); // TODO: Add DI
                var isStillAlive = serviceHelper.IsStillAlive();
                //Console.WriteLine($"{service.ServiceName} is active: {isStillAlive}");
                var item = persistenceService.GetItemById(service.ServiceName);
                if (item == null)
                    item = new DBItem { Id = service.ServiceName, RetryCount = 0 };

                if (!isStillAlive)
                {
                    serviceHelper.Start();
                    item.RetryCount++;
                    if (item.RetryCount > service.MaxRetryCount)
                    {
                        // Console.WriteLine($"Invio messaggio");
                        await notificationService.SendNotificationAsync(service.RecipientAddress, service.FailMessage, "Attention: Service is down");
                        item.RetryCount = 0;
                    }
                    // Console.WriteLine($"{service.ServiceName} is not active: {item.RetryCount}");
                }
                else
                {
                    if (item.RetryCount > 0)
                    {
                        Console.WriteLine($"Invio messaggio");
                        await notificationService.SendNotificationAsync(service.RecipientAddress, service.RestoredMessage, "Good news: Still alive");
                    }
                    item.RetryCount = 0;
                }
                persistenceService.UpdateCachedItem(item);
                persistenceService.PersistOnDisk();
            }
        }
    }
}
