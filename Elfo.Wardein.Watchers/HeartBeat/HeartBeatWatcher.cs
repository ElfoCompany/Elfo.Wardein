using System;
using System.Threading.Tasks;
using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Abstractions.HeartBeat;
using Elfo.Wardein.Core;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.HeartBeat
{
    public class HeartBeatWatcher : WardeinWatcher<HeartbeatConfigurationModel>
    {
        private readonly string heartBeatAppHostname;
        private readonly IAmWardeinHeartBeatPersistanceService heartBeatPersistanceService;


        public HeartBeatWatcher(HeartbeatConfigurationModel config, string name, string group = null) : base(name, config, group, true)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Application name can not be empty", nameof(HeartBeatWatcher));

            heartBeatAppHostname = config.ApplicationHostname;

            heartBeatPersistanceService = ServicesContainer.WardeinHeartBeatPersistenceService();
        }

        public static HeartBeatWatcher Create(HeartbeatConfigurationModel config, string group = null)
        {
            return new HeartBeatWatcher(config, $"{nameof(HeartBeatWatcher)}", group);
        }

        public override async Task<IWatcherCheckResult> ExecuteWatcherActionAsync()
        {
            var result = await heartBeatPersistanceService.UpdateHeartBeat(heartBeatAppHostname);
            return HeartBeatWatcherCheckResult.Create(this, result);
        }
    }
}
