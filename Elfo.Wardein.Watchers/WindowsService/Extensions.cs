using System;
using System.Collections.Generic;
using System.Text;
using Warden;
using Warden.Core;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.WindowsService
{
    public static class Extensions
    {
        public static WardenConfiguration.Builder AddWindowsServiceWatcher(this WardenConfiguration.Builder builder,
            WindowsServiceWatcherConfig config,
            string group = null,
            Action<WatcherHooksConfiguration.Builder> hooks = null)
        {
            builder.AddWatcher(WindowsServiceWatcher.Create(config, group), hooks, TimeSpan.FromSeconds(config.TimeSpanFromSeconds));
            return builder;
        }
    }
}
