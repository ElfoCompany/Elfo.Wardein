using Elfo.Wardein.Abstractions.Configuration.Models;
using System;
using Warden.Core;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.HeartBeat
{
    public static class Extensions
    {
        public static WardenConfiguration.Builder AddWardeinHeartBeatWatcher(this WardenConfiguration.Builder builder,
           HeartbeatConfigurationModel config,
           string group = null,
           Action<WatcherHooksConfiguration.Builder> hooks = null)
        {
            builder.AddWatcher(HeartBeatWatcher.Create(config, group), hooks, TimeSpan.FromSeconds(config.TimeSpanFromSeconds ?? 60));
            return builder;
        }
    }
}
