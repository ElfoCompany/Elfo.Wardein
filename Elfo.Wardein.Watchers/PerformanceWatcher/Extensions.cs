using Elfo.Wardein.Abstractions.Configuration.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Warden.Core;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.PerformanceWatcher
{
    public static class Extensions
    {
        public static WardenConfiguration.Builder AddPerformanceWatcher(this WardenConfiguration.Builder builder,
           PerformanceWatcherConfigurationModel config,
           string group = null,
           Action<WatcherHooksConfiguration.Builder> hooks = null,
           double timeSpanFromSeconds = 60)
        {
            builder.AddWatcher(PerformanceWatcher.Create(config, group), hooks, TimeSpan.FromSeconds(config.TimeSpanFromSeconds ?? timeSpanFromSeconds));
            return builder;
        }
    }
}
