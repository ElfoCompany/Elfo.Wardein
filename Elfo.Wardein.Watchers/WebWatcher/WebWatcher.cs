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
            Log.Info($"---\tStarting {Name}\t---");
            try
            {
                var guid = Guid.NewGuid();
                log.Info($"{Environment.NewLine}{"-".Repeat(24)} UrlWatcher check @ {guid} started {"-".Repeat(24)}");

                result = await RunCheck();

                log.Info($"{Environment.NewLine}{"-".Repeat(24)} UrlWatcher check @ {guid} finished {"-".Repeat(24)}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Exception inside UrlWatcher action: {ex.ToString()}\n");
            }
            return WebWatcherCheckResult.Create(this, result);
        }

        internal virtual async Task<bool> RunCheck()
        {
            log.Info($"Starting check on {GetLoggingDisplayName}");

            var notificationService = ServicesContainer.NotificationService(Config.NotificationType);
            var isHealthy = await urlResponseManager.IsHealthy(Config, Config.Method);
            var currentStatus = await watcherPersistenceService.UpsertCurrentStatus
            (
                watcherConfigurationId: Config.WatcherConfigurationId,
                applicationId: Config.ApplicationId,
                applicationHostname: Config.ApplicationHostname,
                isHealthy: isHealthy
            );

            if (!isHealthy)
            {
                await PerformActionOnServiceDown(currentStatus, async configuration => await urlResponseManager.RestartPool(configuration.AssociatedIISPool));
            }
            else
            {
                await PerformActionOnServiceAlive(currentStatus);
            }

            log.Info($"Finished checking {GetLoggingDisplayName}");
            return isHealthy;
        }

        protected override string GetLoggingDisplayName => string.IsNullOrWhiteSpace(Config.UrlAlias) ? Config.Url.AbsoluteUri : Config.UrlAlias;
    }
}
