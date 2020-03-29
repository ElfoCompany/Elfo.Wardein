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

namespace Elfo.Wardein.Watchers.WebWatcher
{
    public class WebWatcher : WardeinWatcherWithResolution<WebWatcherConfigurationModel>
    {
        private readonly IAmWatcherPersistenceService watcherPersistenceService;
        private readonly IAmUrlResponseManager urlResponseManager;
        protected static ILogger log = LogManager.GetCurrentClassLogger();

        protected WebWatcher(string name, string group, WebWatcherConfigurationModel config) : base(name, config, group)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Watcher name can not be empty.");

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config),
                    "Web Watcher configuration has not been provided.");
            }
            watcherPersistenceService = ServicesContainer.WatcherPersistenceService();
            urlResponseManager = ServicesContainer.UrlResponseManager();
        }

        public static WebWatcher Create(WebWatcherConfigurationModel config, string group = null)
        {
            return new WebWatcher($"{nameof(WebWatcher)}", group, config);
        }

        public override async Task<IWatcherCheckResult> ExecuteWatcherActionAsync()
        {
            bool result = false;
            try
            {
                var guid = Guid.NewGuid();
                log.Debug($"{GetLoggingDisplayName} web check started");

                result = await RunCheck();

                log.Debug($"{GetLoggingDisplayName} web check finished{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Exception inside UrlWatcher action: {ex.ToString()}\n");
            }
            return WebWatcherCheckResult.Create(this, result);
        }

        internal virtual async Task<bool> RunCheck()
        {
            var notificationService = ServicesContainer.NotificationService(Config.NotificationType);
            var isHealthy = await urlResponseManager.IsHealthy(Config);
            log.Info($"{GetLoggingDisplayName} web check isHealthy: {isHealthy}");
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
                        await urlResponseManager.RestartPool(configuration.AssociatedIISPool);
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

        protected override string GetLoggingDisplayName => string.IsNullOrWhiteSpace(Config.UrlAlias) ? Config.Url.AbsoluteUri : Config.UrlAlias;
    }
}
