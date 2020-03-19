using System;
using System.Threading.Tasks;
using Elfo.Firmenich.Wardein.Abstractions.HeartBeat;
using Elfo.Wardein.Core;
using Elfo.Wardein.Watchers.HeartBeat.Config;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.HeartBeat
{
    public class HeartBeatWatcher : WardeinWatcher<HeartBeatWatcherConfig>
    {
        private readonly string heartBeatAppHostname;
        private readonly IAmWardeinHeartBeatPersistanceService heartBeatPersistanceService;


        public HeartBeatWatcher(HeartBeatWatcherConfig config, string name, string group = null) : base(name, config, group )
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Application name can not be empty", nameof(HeartBeatWatcherConfig));

            heartBeatAppHostname = config.HeartBeatAppName;

            heartBeatPersistanceService = ServicesContainer.WardeinHeartBeatPersistenceService(config.ConnectionString);
        }

        public static HeartBeatWatcher Create(HeartBeatWatcherConfig config, string group = null)
        {
            return new HeartBeatWatcher(config, $"{nameof(HeartBeatWatcher)}", group);
        }

        public override async Task<IWatcherCheckResult> ExecuteWatcherActionAsync()
        {
            await heartBeatPersistanceService.UpdateHeartBeat(heartBeatAppHostname);
            return await Task.FromResult(Task.CompletedTask as IWatcherCheckResult);
        }
    }
}
