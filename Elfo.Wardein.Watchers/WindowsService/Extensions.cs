using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Watchers.GenericService;
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
            GenericServiceConfigurationModel config,
            string group = null,
            Action<WatcherHooksConfiguration.Builder> hooks = null,
            double timeSpanFromSeconds = 60)
        {
            builder.AddWatcher(WindowsServiceWatcher.Create(config, group), hooks, TimeSpan.FromSeconds(config.TimeSpanFromSeconds ?? timeSpanFromSeconds));
            return builder;
        }
    }
}
