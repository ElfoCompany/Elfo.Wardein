using Elfo.Firmenich.Wardein.Abstractions.Watchers;
using Elfo.Firmenich.Wardein.Abstractions.WebWatcher;
using Elfo.Firmenich.Wardein.Core.ServiceManager;
using Elfo.Wardein.Core;
using Elfo.Wardein.Core.Helpers;
using Elfo.Wardein.Core.NotificationService;
using NLog;
using NLog.Fluent;
using System;
using System.Threading.Tasks;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.WebWatcher
{
    public class WebWatcher : WardeinWatcher<WebWatcherConfig>
    {
        private readonly WebWatcherConfig configuration;
        private readonly IAmWatcherPersistenceService watcherPersistenceService;
        private readonly HttpClientUrlResponseManager urlResponseManager;
        protected static ILogger log = LogManager.GetCurrentClassLogger();

        protected WebWatcher(string name, WebWatcherConfig config, string group) : base(name, config, group)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Watcher name can not be empty.");

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration),
                    "Web Watcher configuration has not been provided.");
            }

            configuration = config;
            watcherPersistenceService = ServicesContainer.WatcherPersistenceService(configuration.ConnectionString);

        }

        public override async Task<IWatcherCheckResult> ExecuteWatcherActionAsync()
        {
            Log.Info($"---\tStarting {Name}\t---");
            try
            {
                var guid = Guid.NewGuid();
                log.Info($"{Environment.NewLine}{"-".Repeat(24)} UrlWatcher check @ {guid} started {"-".Repeat(24)}");

                await RunCheck();

                log.Info($"{Environment.NewLine}{"-".Repeat(24)} UrlWatcher check @ {guid} finished {"-".Repeat(24)}{Environment.NewLine.Repeat(24)}");
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Exception inside UrlWatcher action: {ex.ToString()}\n");
            }

            return await Task.FromResult<IWatcherCheckResult>(null);
        }

        internal virtual async Task RunCheck()
        {
            log.Info($"Starting check on {GetUrlDisplayName}");

            var notificationService = ServicesContainer.NotificationService(NotificationType.Mail);
            var isHealthy = await urlResponseManager.IsHealthy(configuration.AssertWithStatusCode, configuration.AssertWithRegex, configuration.Url);
            var currentStatus = await watcherPersistenceService.UpsertCurrentStatus
            (
                watcherConfigurationId: configuration.WatcherConfigurationId,
                applicationId: configuration.ApplicationId,
                applicationHostname: configuration.ApplicationHostname,
                isHealthy: isHealthy
            );

            if (!isHealthy)
            {
                await PerformActionOnServiceDown(configuration.AssociatedIISPool);
            }
            else
            {
                await PerformActionOnServiceAlive();
            }

            log.Info($"Finished checking {GetUrlDisplayName}");
            #region Local Functions

            async Task PerformActionOnServiceDown(string poolName)
            {
                if (IsFailureCountEqualToMaxRetyrCount() || IsMultipleOfReminderRetryCount())
                {
                    log.Warn($"Sending Fail Notification for {GetUrlDisplayName}");
                    await notificationService.SendNotificationAsync(configuration.RecipientAddresses, configuration.FailureMessage,
                            $"Attention: {GetUrlDisplayName} is down on {configuration.ApplicationHostname}");
                }

                if (string.IsNullOrWhiteSpace(poolName))
                {
                    log.Info($"trying to restore {poolName} on {configuration.ApplicationHostname}");
                    await urlResponseManager.RestartPool(poolName);
                    log.Info($"{poolName} was restarted");
                }

                #region Local Functions

                bool IsFailureCountEqualToMaxRetyrCount() => currentStatus.FailureCount == configuration.MaxRetryCount;

                bool IsMultipleOfReminderRetryCount() => currentStatus.FailureCount % configuration.SendReminderEmailAfterRetryCount == 0;

                #endregion
            }

            async Task PerformActionOnServiceAlive()
            {
                try
                {
                    log.Info($"{GetUrlDisplayName} is active");
                    if (!currentStatus.PreviousStatus)
                    {
                        log.Info($"Send Restored Notification for {GetUrlDisplayName}");
                        await notificationService.SendNotificationAsync(configuration.RecipientAddresses, configuration.RestoredMessage,
                              $"Good news: {GetUrlDisplayName} has been restored succesfully");
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex, $"Unable to send email fro {GetUrlDisplayName}");
                }
            }
            #endregion
        }

        private string GetUrlDisplayName => string.IsNullOrWhiteSpace(configuration.UrlAlias) ? configuration.Url.AbsoluteUri : configuration.UrlAlias;
    }
}
