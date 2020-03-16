using System;
using System.Collections.Generic;
using System.Text;
using Warden;
using Warden.Core;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.FileSystem
{
    public static class Extensions
    {
        public static WardenConfiguration.Builder AddFileSystemWatcher(this WardenConfiguration.Builder builder,
            FileSystemWatcherConfig config,
            string group = null,
            Action<WatcherHooksConfiguration.Builder> hooks = null)
        {
            builder.AddWatcher(FileSystemWatcher.Create(config, group), hooks, TimeSpan.FromSeconds(config.TimeSpanFromSeconds));
            return builder;
        }
    }
}
