using Elfo.Wardein.Abstractions.Configuration;
using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Core;
using NLog;
using System.Threading.Tasks;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers
{
    public abstract class WardeinWatcher<TConfig> : IWatcher where TConfig : IAmBaseConfigurationModel
    {
        private readonly IAmWardeinConfigurationManager wardeinConfigurationManager;
        private readonly bool skipMaintenanceMode;

        protected WardeinWatcher(string name, TConfig config, string group = null, bool skipMaintenanceMode = false)
        {
            Name = name;
            Group = group;
            this.skipMaintenanceMode = skipMaintenanceMode;
            Config = config;
            wardeinConfigurationManager = ServicesContainer.WardeinConfigurationManager();
        }

        public virtual string Name { get; protected set; }

        public virtual string Group { get; protected set; }

        public TConfig Config { get; protected set; }

        protected static ILogger log = LogManager.GetCurrentClassLogger();

        public async Task<IWatcherCheckResult> ExecuteAsync()
        {
            if (!skipMaintenanceMode && wardeinConfigurationManager.IsInMaintenanceMode)
            {
                var message = $"Wardein {Name} running in maintainance mode. Skipping Execution.";
                log.Debug(message);
                return await Task.FromResult(WatcherCheckResult.Create(this, true, message));
            }

            return await ExecuteWatcherActionAsync();
        }

        public abstract Task<IWatcherCheckResult> ExecuteWatcherActionAsync();
    }
}
