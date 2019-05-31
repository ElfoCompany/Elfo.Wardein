using Elfo.Wardein.Core.ConfigurationReader;
using Elfo.Wardein.Core.Helpers;
using Elfo.Wardein.Core.Models;
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
using NLog;
using Elfo.Wardein.Core.ServiceManager;

namespace Elfo.Wardein.Core
{
    public class WardeinInstance
    {
        #region Private variables

        private WardeinConfig wardeinConfig = null;
        private readonly static Logger log = LogManager.GetCurrentClassLogger();
        private readonly IAmWardeinConfigurationManager wardeinConfigurationReader;

        #endregion

        #region Constructor

        public WardeinInstance()
        {
            this.wardeinConfigurationReader = ServicesContainer.WardeinConfigurationManager(Const.WARDEIN_CONFIG_PATH);

            GetWarderinConfigAndThrowErrorIfNotExist();

            #region Local Functions

            void GetWarderinConfigAndThrowErrorIfNotExist()
            {
                if (!File.Exists(Const.WARDEIN_CONFIG_PATH))
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
            log.Info($"{Environment.NewLine}> CHECKING SERVICES HEALTH");

            if (this.wardeinConfigurationReader.IsInMaintenanceMode)
            {
                log.Info("Wardein is in maintenance mode.");
                return;
            }

            foreach (var service in wardeinConfig.Services)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(250)); // TODO: Do we really need this?

                using (var persistenceService = ServicesContainer.PersistenceService(Const.DB_PATH))
                {
                    IAmServiceManager serviceManager = GetServiceManager();
                    if (serviceManager == null)
                        continue; // If the service doesn't exist, skip the check 

                    var notificationService = ServicesContainer.NotificationService(GetNotificationType());
                    var item = persistenceService.GetEntityById(service.ServiceName);

                    if (!serviceManager.IsStillAlive)
                    {
                        await PerformActionOnServiceDown();
                    }
                    else
                    {
                        await PerformActionOnServiceAlive();
                    }

                    persistenceService.CreateOrUpdateCachedEntity(item);


                    #region Local Functions

                    IAmServiceManager GetServiceManager()
                    {
                        IAmServiceManager svc = null;
                        try
                        {
                            svc = ServicesContainer.ServiceManager(service.ServiceName, ServiceManagerType.WindowsService);
                        }
                        catch (ArgumentNullException ex)
                        {
                            log.Warn(ex.Message);
                        }
                        return svc;
                    }

                    NotificationType GetNotificationType()
                    {
                        if (!Enum.TryParse<NotificationType>(service.NotificationType, out NotificationType result))
                            throw new ArgumentException($"Notification type {service.NotificationType} not supported");
                        return result;
                    }

                    async Task PerformActionOnServiceDown()
                    {
                        serviceManager.Start();
                        item.RetryCount++;

                        if (IsRetryCountExceededOrEqual() && IsMultipleOfMaxRetryCount())
                        {
                            if (IAmAllowedToSendANewNotification())
                            {
                                log.Warn($"Sending Fail Notification");
                                await notificationService.SendNotificationAsync(service.RecipientAddress, service.FailMessage, $"Attention: {service.ServiceName} service is down");
                                item.LastNotificationSentAtThisTimeUTC = DateTime.UtcNow;
                            }
                        }
                        log.Info($"{service.ServiceName} is not active, retry: {item.RetryCount}");

                        #region Local Functions

                        bool IAmAllowedToSendANewNotification()
                        {
                            if (item.RetryCount <= NumberOfNotificationAllowedAtMaximumRate() || NeverSentANotificationBefore())
                                return true;

                            return IsRepeatedMailTimeoutElapsed();

                            #region Local Functions

                            int NumberOfNotificationAllowedAtMaximumRate() => service.MaxRetryCount * GetServiceNumberOfNotificationWithoutRateLimitationOrDefault();

                            bool IsRepeatedMailTimeoutElapsed()
                            {
                                var timeout = GetServiceSendRepeatedNotificationAfterSecondsOrDefault();

                                return DateTime.UtcNow.Subtract(item.LastNotificationSentAtThisTimeUTC.GetValueOrDefault(DateTime.MinValue)) >= timeout;
                            }

                            bool NeverSentANotificationBefore() => item.LastNotificationSentAtThisTimeUTC.HasValue == false; 

                            #endregion
                        }

                        bool IsRetryCountExceededOrEqual() => item.RetryCount >= service.MaxRetryCount;

                        bool IsMultipleOfMaxRetryCount() => item.RetryCount % service.MaxRetryCount == 0;

                        #endregion
                    }

                    async Task PerformActionOnServiceAlive()
                    {
                        try
                        {
                            log.Info($"{service.ServiceName} is active");
                            if (item.RetryCount > 0)
                            {
                                log.Info($"Send Restored Notification");
                                await notificationService.SendNotificationAsync(service.RecipientAddress, service.RestoredMessage, $"Good news: {service.ServiceName} service has been restored succesfully");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex, "Unable to send email");
                        }
                        finally
                        {
                            item.RetryCount = 0;
                        }
                    }

                    TimeSpan GetServiceSendRepeatedNotificationAfterSecondsOrDefault() =>
                        TimeSpan.FromSeconds(service.SendRepeatedNotificationAfterSeconds.GetValueOrDefault(wardeinConfig.SendRepeatedNotificationAfterSeconds));

                    int GetServiceNumberOfNotificationWithoutRateLimitationOrDefault()
                    {
                        var result = service.NumberOfNotificationsWithoutRateLimitation.GetValueOrDefault(wardeinConfig.NumberOfNotificationsWithoutRateLimitation);
                        if (result <= 0)
                            return int.MaxValue;

                        return result;
                    }

                    #endregion
                }
            }
        }
    }
}
