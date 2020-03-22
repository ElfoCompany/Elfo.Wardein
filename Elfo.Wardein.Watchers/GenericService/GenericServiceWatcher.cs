using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Abstractions.Services;
using Elfo.Wardein.Abstractions.Watchers;
using Elfo.Wardein.Core;
using Elfo.Wardein.Core.Helpers;
using Elfo.Wardein.Core.NotificationService;
using NLog.Fluent;
using System;
using System.Threading.Tasks;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.GenericService
{
    public abstract class GenericServiceWatcher : WardeinWatcherWithResolution<GenericServiceConfigurationModel>
    {
        private readonly IAmServiceManager serviceManager;
        private readonly IAmWatcherPersistenceService watcherPersistenceService;

        internal GenericServiceWatcher(GenericServiceConfigurationModel config, string name, string group = null) : base(name, config, group)
        {
            serviceManager = GetServiceManager();
            watcherPersistenceService = ServicesContainer.WatcherPersistenceService();
        }

        public string ServiceDisplayType => Config.ServiceManagerType.ToString();

        public override async Task<IWatcherCheckResult> ExecuteWatcherActionAsync()
        {
            bool result = false;
            Log.Info($"---\tStarting {Name}\t---");
            try
            {
                var guid = Guid.NewGuid();
                log.Info($"{Environment.NewLine}{"-".Repeat(24)} {ServiceDisplayType} health check on {Config.ServiceName} @ {guid} started {"-".Repeat(24)}");

                result = await RunCheck();

                log.Info($"{Environment.NewLine}{"-".Repeat(24)} {ServiceDisplayType} health check on {Config.ServiceName} @ {guid} finished {"-".Repeat(24)}{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Exception inside polling action: {ex.ToString()}\n");
            }

            return WatcherCheckResult.Create(this, result);
        }

        internal virtual async Task<bool> RunCheck()
        {
            log.Info($"{Environment.NewLine}> CHECKING SERVICES HEALTH");
            
            var notificationService = ServicesContainer.NotificationService(Config.NotificationType);
            var isHealthy = await serviceManager.IsStillAlive();
            var currentStatus = await watcherPersistenceService.UpsertCurrentStatus
            (
                watcherConfigurationId: Config.WatcherConfigurationId,
                applicationId: Config.ApplicationId,
                applicationHostname: Config.ApplicationHostname,
                isHealthy: isHealthy
            );

            if (!isHealthy)
            {
                await PerformActionOnServiceDown(currentStatus, async (config) => await serviceManager.Restart());
            }
            else
            {
                await PerformActionOnServiceAlive(currentStatus);
            }
            return await Task.FromResult(false);
        }

        protected virtual IAmServiceManager GetServiceManager()
        {
            IAmServiceManager svc = null;
            try
            {
                svc = ServicesContainer.ServiceManager(Config.ServiceName, Config.ServiceManagerType);
            }
            catch (ArgumentNullException ex)
            {
                log.Warn(ex.Message);
            }
            return svc;
        }
    }
}
