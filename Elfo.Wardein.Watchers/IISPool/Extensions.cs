using System;
using System.Collections.Generic;
using System.Text;
using Warden.Core;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.IISPool
{
    public static class Extensions
    {
        public static WardenConfiguration.Builder AddIISPoolWatcher(this WardenConfiguration.Builder builder,
            IISPoolWatcherConfig config,
            string group = null,
            Action<WatcherHooksConfiguration.Builder> hooks = null)
        {
            builder.AddWatcher(IISPoolWatcher.Create(config, group), hooks, TimeSpan.FromSeconds(config.TimeSpanFromSeconds));
            return builder;
        }
    }
}
