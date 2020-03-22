using Elfo.Wardein.Abstractions.Configuration.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Warden.Core;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.WebWatcher
{
    public static class Extensions
    {
        public static WardenConfiguration.Builder AddWebWatcher(this WardenConfiguration.Builder builder,
           WebWatcherConfigurationModel config,
           string group = null,
           Action<WatcherHooksConfiguration.Builder> hooks = null)
        {
            builder.AddWatcher(WebWatcher.Create(config, group), hooks, TimeSpan.FromSeconds(config.TimeSpanFromSeconds));
            return builder;
        }
    }
}
