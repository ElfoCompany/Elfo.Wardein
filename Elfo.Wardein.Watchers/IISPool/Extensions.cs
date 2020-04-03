using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Watchers.GenericService;
using System;
using Warden.Core;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.IISPool
{
    public static class Extensions
    {
        public static WardenConfiguration.Builder AddIISPoolWatcher(this WardenConfiguration.Builder builder,
            GenericServiceConfigurationModel config,
            string group = null,
            Action<WatcherHooksConfiguration.Builder> hooks = null,
            double timeSpanFromSeconds = 60)
        {
            builder.AddWatcher(IISPoolWatcher.Create(config, group), hooks, TimeSpan.FromSeconds(config.TimeSpanFromSeconds ?? timeSpanFromSeconds));
            return builder;
        }
    }
}
