using Elfo.Wardein.Core;
using NLog;
using System;
using System.Threading.Tasks;
using Warden.Watchers;
using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Watchers.BaseUrlWatcher;

namespace Elfo.Wardein.Watchers.WebWatcher
{
    public class WebWatcher : BaseUrlWatcher<WebWatcherConfigurationModel>
    {
        protected static ILogger log = LogManager.GetCurrentClassLogger();
        private const string ConfigurationNotProvidedMessage = "Web Watcher configuration has not been provided.";

        protected WebWatcher(string name, string group, WebWatcherConfigurationModel config) 
            : base(name, config, ConfigurationNotProvidedMessage, ServicesContainer.UrlResponseManager(), group) { }

        public static WebWatcher Create(WebWatcherConfigurationModel config, string group = null)
        {
            return new WebWatcher($"{nameof(WebWatcher)}", group, config);
        }

        public override async Task<IWatcherCheckResult> ExecuteWatcherActionAsync()
        {
            bool result = false;
            try
            {
                var guid = Guid.NewGuid();
                log.Debug($"{GetLoggingDisplayName} web check started");

                result = await RunCheck();

                log.Debug($"{GetLoggingDisplayName} web check finished{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Exception inside UrlWatcher action: {ex.ToString()}\n");
            }
            return WebWatcherCheckResult.Create(this, result);
        }

        protected override string GetLoggingDisplayName => string.IsNullOrWhiteSpace(Config.UrlAlias) ? Config.Url.AbsoluteUri : Config.UrlAlias;
    }
}
