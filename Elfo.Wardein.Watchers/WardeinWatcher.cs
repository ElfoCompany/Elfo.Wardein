using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers
{
    public abstract class WardeinWatcher<TConfig> : IWatcher where TConfig : IWatcherConfig
    {
        protected WardeinWatcher(string name, TConfig config, string group = null)
        {
            Name = name;
            Group = group;
            Config = config;
        }

        public virtual string Name { get; protected set; }

        public virtual string Group { get; protected set; }

        public TConfig Config { get; protected set; }

        protected static ILogger log = LogManager.GetCurrentClassLogger();

        public async Task<IWatcherCheckResult> ExecuteAsync()
        {
            if (Config.IsInMaintenanceMode)
            {
                var message = $"Wardein {Name} running in maintainance mode. Skipping Execution.";
                log.Info(message);
                return await Task.FromResult(WatcherCheckResult.Create(this, true, message));
            }

            return await ExecuteWatcherActionAsync();
        }

        public abstract Task<IWatcherCheckResult> ExecuteWatcherActionAsync();
    }
}
