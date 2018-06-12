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
using Microsoft.Extensions.DependencyInjection;

namespace Elfo.Wardein.Core
{
    public class WardeinInstance
    {
        #region Private variables

        private string configPath = $@"{Const.BASE_PATH}Assets\WardeinConfig.json";
        private string dbPath = $@"{Const.BASE_PATH}Assets\WardeinDB.json";
        private WardeinConfig wardeinConfig = null;

        private readonly IAmWardeinConfigurationReaderService wardeinConfigurationReader;        

        #endregion

        #region Constructor

        public WardeinInstance()
        {
            this.wardeinConfigurationReader = ServicesContainer.WardeinConfigurationReaderService(configPath);            

            GetWarderinConfigAndThrowErrorIfNotExist();

            #region Local Functions

            void GetWarderinConfigAndThrowErrorIfNotExist()
            {
                if (!File.Exists(configPath))
                {
                    //TODO: throw error or something...
                }
                else
                {
                    this.wardeinConfig = wardeinConfigurationReader.GetConfiguration();
                    if (this.wardeinConfig == null)
                        throw new ArgumentNullException("Wardein configuration not found or not well formatted");
                }


            }

            #endregion
        }

        #endregion

        public async Task RunCheck()
        {
            foreach (var service in wardeinConfig.Services)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(250)); // TODO: Do we really need this?

                using (var persistenceService = ServicesContainer.PersistenceService(dbPath))
                {
                    var serviceHelper = new WindowsServiceHelper(service.ServiceName);
                    var notificationService = ServicesContainer.NotificationService(GetNotificationType());
                    var item = persistenceService.GetEntityById(service.ServiceName);

                    if (!serviceHelper.IsStillAlive)
                    {
                        await PerformActionOnServiceDown();
                    }
                    else
                    {
                        await PerformActionOnServiceRestored();
                    }

                    persistenceService.CreateOrUpdateCachedEntity(item);


                    #region Local Functions

                    NotificationType GetNotificationType()
                    {
                        if (!Enum.TryParse<NotificationType>(service.NotificationType, out NotificationType result))
                            throw new ArgumentException($"Notification type {service.NotificationType} not supported");
                            return result;
                    }

                    async Task PerformActionOnServiceDown()
                    {
                        Console.WriteLine($"{service.ServiceName} is not active");
                        serviceHelper.Start();
                        item.RetryCount++;
                        if (item.RetryCount > service.MaxRetryCount)
                        {
                            Console.WriteLine($"Send Fail Notification");
                            await notificationService.SendNotificationAsync(service.RecipientAddress, service.FailMessage, "Attention: Service is down");
                            item.RetryCount = 0;
                        }
                        Console.WriteLine($"{service.ServiceName} is not active: {item.RetryCount}");                        
                    }

                    async Task PerformActionOnServiceRestored()
                    {
                        Console.WriteLine($"{service.ServiceName} is now active");
                        if (item.RetryCount > 0)
                        {
                            Console.WriteLine($"Send Restored Notification");
                            await notificationService.SendNotificationAsync(service.RecipientAddress, service.RestoredMessage, "Good news: Still alive");
                        }
                        item.RetryCount = 0;
                    }

                    #endregion
                }
            }
        }
    }
}
