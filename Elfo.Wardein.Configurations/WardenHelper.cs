using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Watchers.FileSystem;
using Elfo.Wardein.Watchers.HeartBeat;
using Elfo.Wardein.Watchers.IISPool;
using Elfo.Wardein.Watchers.PerformanceWatcher;
using Elfo.Wardein.Watchers.WebWatcher;
using Elfo.Wardein.Watchers.WindowsService;
using System.Linq;
using Warden.Core;

namespace Elfo.Wardein.Configurations
{
    public static class WardenHelper
    {
        public static WardenConfiguration.Builder GetWardenConfigurationBuilder(WardeinConfig wardeinConfiguration)
        {
            var configurationBuilder = WardenConfiguration.Create();

            configurationBuilder.ConfigureWardenConfigurationBuilder(wardeinConfiguration);

            return configurationBuilder;
        }

        public static WardenConfiguration.Builder ClearWatchers(this WardenConfiguration.Builder configurationBuilder)
        {
            try
            {
                configurationBuilder.RemoveWatcher(nameof(HeartBeatWatcher));
            }
            catch { }

            try
            {
                configurationBuilder.RemoveWatcher(nameof(PerformanceWatcher));
            }
            catch { }

            try
            {
                configurationBuilder.RemoveWatcher(nameof(WebWatcher));
            }
            catch { }

            try
            {
                configurationBuilder.RemoveWatcher(nameof(WindowsServiceWatcher));
            }
            catch { }

            try
            {
                configurationBuilder.RemoveWatcher(nameof(IISPoolWatcher));
            }
            catch { }

            try
            {
                configurationBuilder.RemoveWatcher(nameof(FileSystemWatcher));
            }
            catch { }




            return configurationBuilder;
        }

        public static WardenConfiguration.Builder ConfigureWardenConfigurationBuilder(this WardenConfiguration.Builder configurationBuilder, WardeinConfig wardeinConfiguration)
        {
            configurationBuilder
                .AddWardeinHeartBeatWatcher(wardeinConfiguration.Heartbeat, "HeartbeatWatcher");

            if (wardeinConfiguration.Performances?.Count() > 0)
                foreach (var performance in wardeinConfiguration.Performances)
                    configurationBuilder.AddPerformanceWatcher(performance, "PerformanceWatcher", timeSpanFromSeconds: wardeinConfiguration.TimeSpanFromSeconds);

            if (wardeinConfiguration.Urls?.Count() > 0)
                foreach (var url in wardeinConfiguration.Urls)
                    configurationBuilder.AddWebWatcher(url, "WebWatcher", timeSpanFromSeconds: wardeinConfiguration.TimeSpanFromSeconds);

            if (wardeinConfiguration.Services?.Count() > 0)
                foreach (var service in wardeinConfiguration.Services)
                    configurationBuilder.AddWindowsServiceWatcher(service, "ServiceWatcher", timeSpanFromSeconds: wardeinConfiguration.TimeSpanFromSeconds);

            if (wardeinConfiguration.IISPools?.Count() > 0)
                foreach (var pool in wardeinConfiguration.IISPools)
                    configurationBuilder.AddIISPoolWatcher(pool, "WebWatcher", timeSpanFromSeconds: wardeinConfiguration.TimeSpanFromSeconds);

            if (wardeinConfiguration.CleanUps?.Count() > 0)
                foreach (var cleanUp in wardeinConfiguration.CleanUps)
                    configurationBuilder.AddFileSystemWatcher(cleanUp, "CleanUpWatcher", timeSpanFromSeconds: wardeinConfiguration.TimeSpanFromSeconds);


            return configurationBuilder;
        }


    }
}
