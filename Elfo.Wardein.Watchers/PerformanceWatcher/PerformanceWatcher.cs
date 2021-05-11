using Elfo.Wardein.Abstractions.Watchers;
using Elfo.Wardein.Abstractions.WebWatcher;
using Elfo.Wardein.Core;
using Elfo.Wardein.Core.Helpers;
using NLog;
using NLog.Fluent;
using System;
using System.Threading.Tasks;
using Warden.Watchers;
using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Abstractions.PerformanceWatcher;

namespace Elfo.Wardein.Watchers.PerformanceWatcher
{
    public class PerformanceWatcher : WardeinWatcherWithResolution<PerformanceWatcherConfigurationModel>
    {
        private readonly IAmWatcherPersistenceService watcherPersistenceService;
        private readonly IAmUrlPerformanceManager urlPerformanceManager;
        protected static ILogger log = LogManager.GetCurrentClassLogger();

        protected PerformanceWatcher(string name, string group, PerformanceWatcherConfigurationModel config) : base(name, config, group)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Watcher name can not be empty.");

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config),
                    "Performance Watcher configuration has not been provided.");
            }
            watcherPersistenceService = ServicesContainer.WatcherPersistenceService();
            urlPerformanceManager = ServicesContainer.UrlPerformanceManager();
        }

        public static PerformanceWatcher Create(PerformanceWatcherConfigurationModel config, string group = null)
        {
            return new PerformanceWatcher($"{nameof(PerformanceWatcher)}", group, config);
        }

        public override async Task<IWatcherCheckResult> ExecuteWatcherActionAsync()
        {
            bool result = false;
            try
            {
                var guid = Guid.NewGuid();
                log.Debug($"{GetLoggingDisplayName} performance check started");

                result = await RunCheck();

                log.Debug($"{GetLoggingDisplayName} performance check finished{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Exception inside UrlPerformanceWatcher action: {ex.ToString()}\n");
            }
            return PerformanceWatcherCheckResult.Create(this, result);
        }

        internal virtual async Task<bool> RunCheck()
        {
            var notificationService = ServicesContainer.NotificationService(Config.NotificationType);
            var isHealthy = await urlPerformanceManager.IsHealthy(Config);
            log.Info($"{GetLoggingDisplayName} performance check isHealthy: {isHealthy}");
            var currentStatus = await watcherPersistenceService.UpsertCurrentStatus
            (
                watcherConfigurationId: Config.WatcherConfigurationId,
                applicationId: Config.ApplicationId,
                applicationHostname: Config.ApplicationHostname,
                isHealthy: isHealthy
            );

            if (!isHealthy)
            {
                await PerformActionOnServiceDown(currentStatus, async (configuration) => {
                    if (!string.IsNullOrWhiteSpace(configuration.AssociatedIISPool))
                        await urlPerformanceManager.RestartPool(configuration.AssociatedIISPool);
                    else
                        log.Debug($"UrlPerformanceWatcher check @ {GetLoggingDisplayName} can't restart assoicated IIS Pool cause it is not specified");
                });
            }
            else
            {
                await PerformActionOnServiceAlive(currentStatus);
            }

            log.Debug($"Finished checking {GetLoggingDisplayName}");
            return isHealthy;
        }

        protected override string GetLoggingDisplayName => string.IsNullOrWhiteSpace(Config.UrlAlias) ? Config.Url.AbsoluteUri : Config.UrlAlias;
    }
}
