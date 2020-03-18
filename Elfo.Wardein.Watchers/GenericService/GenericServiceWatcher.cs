using Elfo.Wardein.Abstractions.Services;
using Elfo.Wardein.Core;
using Elfo.Wardein.Core.Helpers;
using Elfo.Wardein.Core.NotificationService;
using NLog.Fluent;
using System;
using System.Threading.Tasks;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.GenericService
{
    public abstract class GenericServiceWatcher : WardeinWatcher<GenericServiceWatcherConfig>
    {

        internal GenericServiceWatcher(GenericServiceWatcherConfig config, string name, string group = null) : base(name, config, group)
        {
        }

        public override async Task<IWatcherCheckResult> ExecuteWatcherActionAsync()
        {
            Log.Info($"---\tStarting {Name}\t---");
            try
            {
                var guid = Guid.NewGuid();
                log.Info($"{Environment.NewLine}{"-".Repeat(24)} Services health check @ {guid} started {"-".Repeat(24)}");

                await RunCheck();

                log.Info($"{Environment.NewLine}{"-".Repeat(24)} Services health check @ {guid} finished {"-".Repeat(24)}{Environment.NewLine.Repeat(24)}");
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Exception inside polling action: {ex.ToString()}\n");
            }

            return await Task.FromResult<IWatcherCheckResult>(null);
        }

        internal virtual async Task RunCheck()
        {
            log.Info($"{Environment.NewLine}> CHECKING SERVICES HEALTH");

            foreach (var service in Config.Services)
            {
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
                            svc = ServicesContainer.ServiceManager(service.ServiceName, service.ServiceManagerType);
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
                        TimeSpan.FromSeconds(service.SendRepeatedNotificationAfterSeconds.GetValueOrDefault(Config.SendRepeatedNotificationAfterSeconds));

                    int GetServiceNumberOfNotificationWithoutRateLimitationOrDefault()
                    {
                        var result = service.NumberOfNotificationsWithoutRateLimitation.GetValueOrDefault(Config.NumberOfNotificationsWithoutRateLimitation);
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
