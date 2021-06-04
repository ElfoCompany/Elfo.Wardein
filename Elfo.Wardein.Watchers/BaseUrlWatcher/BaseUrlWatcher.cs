using Elfo.Wardein.Abstractions.BaseUrlWatcher;
using Elfo.Wardein.Abstractions.Configuration.Models.WatcherModels;
using Elfo.Wardein.Abstractions.Watchers;
using Elfo.Wardein.Core;
using System;
using System.Threading.Tasks;

namespace Elfo.Wardein.Watchers.BaseUrlWatcher
{
    public abstract class BaseUrlWatcher<T> : WardeinWatcherWithResolution<T> where T : BaseUrlWatcherConfigurationModel
    {
        protected readonly IAmWatcherPersistenceService watcherPersistenceService;
        protected readonly IAmUrlManager<T> urlManager;
        private const string WebCheckLogWord = "web";
        private const string PerfomanceCheckLogWord = "perfomance";

        protected BaseUrlWatcher(string name, T config, string configErrorMessage, IAmUrlManager<T> urlManager, string group = null) : base(name, config, group)
        {
            ValidateWatcherName(name);
            ValidateWatcherConfigWithMessage(config, configErrorMessage);
            watcherPersistenceService = ServicesContainer.WatcherPersistenceService();
            this.urlManager = urlManager;
        }

        protected virtual void ValidateWatcherName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Watcher name can not be empty.");
        }

        protected virtual void ValidateWatcherConfigWithMessage(T config, string errorMessage)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config), errorMessage);
        }

        internal virtual async Task<bool> RunCheck()
        {
            bool isWebWatcher = this is WebWatcher.WebWatcher;
            //bool isPerfomanceWatcher = this is PerformanceWatcher.PerformanceWatcher;
            var checkLogWord = isWebWatcher ? WebCheckLogWord : PerfomanceCheckLogWord;
            var notificationService = ServicesContainer.NotificationService(Config.NotificationType);
            var isHealthy = await urlManager.IsHealthy(Config);
            log.Info($"{GetLoggingDisplayName} {checkLogWord} check isHealthy: {isHealthy}");
            var currentStatus = await watcherPersistenceService.UpsertCurrentStatus
            (
                watcherConfigurationId: Config.WatcherConfigurationId,
                applicationId: Config.ApplicationId,
                applicationHostname: Config.ApplicationHostname,
                isHealthy: isHealthy
            );

            if (!isHealthy)
            {
                await PerformActionOnServiceDown(currentStatus, async (configuration) =>
                {
                    if (!string.IsNullOrWhiteSpace(configuration.AssociatedIISPool))
                    {
                        try
                        {
                            await urlManager.RestartPool(configuration.AssociatedIISPool);
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex, $"BaseUrlWatcher failed to restart pool {configuration.AssociatedIISPool}");
                        }
                    }
                    else
                        log.Debug($"UrlWatcher check @ {GetLoggingDisplayName} can't restart assoicated IIS Pool cause it is not specified");
                });
            }
            else
            {
               await PerformActionOnServiceAlive(currentStatus);
            }

            log.Debug($"Finished checking {GetLoggingDisplayName}");
            return isHealthy;
        }
    }
}
