using Elfo.Wardein.Watchers.GenericService;
using Elfo.Wardein.Watchers.HeartBeat.Config;
using System;
using Warden.Core;
using Warden.Integrations;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.HeartBeat
{
    public static class Extensions
    {
        public static WardenConfiguration.Builder AddWardeinHeartBeatWatcher(this WardenConfiguration.Builder builder,
           HeartBeatWatcherConfig config,
           string group = null,
           Action<WatcherHooksConfiguration.Builder> hooks = null)
        {
            builder.AddWatcher(HeartBeatWatcher.Create(config, group), hooks, TimeSpan.FromSeconds(config.TimeSpanFromSeconds));
            return builder;
        }
    }
}
