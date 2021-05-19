using Elfo.Wardein.Core;
using NLog;
using System;
using System.Threading.Tasks;
using Warden.Watchers;
using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Watchers.BaseUrlWatcher;

namespace Elfo.Wardein.Watchers.PerformanceWatcher
{
    public class PerformanceWatcher : BaseUrlWatcher<PerformanceWatcherConfigurationModel>
    {
        protected static ILogger log = LogManager.GetCurrentClassLogger();
        private const string ConfigurationNotProvidedMessage = "Performance Watcher configuration has not been provided.";

        protected PerformanceWatcher(string name, string group, PerformanceWatcherConfigurationModel config) 
            : base(name, config, ConfigurationNotProvidedMessage, ServicesContainer.UrlPerformanceManager(), group) { }

        public static PerformanceWatcher Create(PerformanceWatcherConfigurationModel config, string group = null)
        {
            return new PerformanceWatcher($"{nameof(PerformanceWatcher)}", group, config);
        }

        public override async Task<IWatcherCheckResult> ExecuteWatcherActionAsync()
        {
            bool result = false;
            try
            {
                var guid = Guid.NewGuid();
                log.Debug($"{GetLoggingDisplayName} performance check started");

                result = await RunCheck();

                log.Debug($"{GetLoggingDisplayName} performance check finished{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Exception inside UrlPerformanceWatcher action: {ex.ToString()}\n");
            }
            return PerformanceWatcherCheckResult.Create(this, result);
        }

        protected override string GetLoggingDisplayName => string.IsNullOrWhiteSpace(Config.UrlAlias) ? Config.Url.AbsoluteUri : Config.UrlAlias;
    }
}
