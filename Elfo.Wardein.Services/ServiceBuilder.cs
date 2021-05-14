using Elfo.Wardein.APIs;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Warden;
using Warden.Core;
using Microsoft.Extensions.Configuration;
using Elfo.Wardein.Integrations;
using Elfo.Wardein.Watchers.FileSystem;
using Elfo.Wardein.Watchers.WindowsService;
using Elfo.Wardein.Watchers.IISPool;
using Elfo.Wardein.Watchers.HeartBeat;
using Elfo.Wardein.Core.ConfigurationManagers;
using Elfo.Wardein.Core.Helpers;
using System.Linq;
using Elfo.Wardein.Watchers.GenericService;
using Elfo.Wardein.Core;
using Elfo.Wardein.Watchers.WebWatcher;
using NLog;

namespace Elfo.Wardein.Services
{
    public class ServiceBuilder
    {
        static IWarden warden;
        protected static ILogger log = LogManager.GetCurrentClassLogger();

        public async Task ConfigureAndRunWarden()
        {
            var wardeinConfigurationManager = ServicesContainer.WardeinConfigurationManager();
            var wardeinConfiguration = wardeinConfigurationManager.GetConfiguration();

            var configurationBuilder = WardenConfiguration
                .Create()
                .SetHooks(hooks =>
                {
                    hooks.OnStart(() => ConfigureAndRunAspNetAPIHosting());
                    hooks.OnIterationStart(n => log.Debug($"Iteration n°{n} started{"-".Repeat(5)}{Environment.NewLine.Repeat(2)}"));
                    hooks.OnIterationCompleted(iteration => log.Debug($"Iteration completed{"-".Repeat(5)}{Environment.NewLine.Repeat(2)}"));
                })
                .AddWardeinHeartBeatWatcher(wardeinConfiguration.Heartbeat, "HeartbeatWatcher");

            // TODO: refactor and add oracle integration (even if useles at the moment)

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

            warden = WardenInstance.Create(configurationBuilder.Build());
            await warden.StartAsync();
        }

        public async Task ConfigureAndRunAPIHosting()
        {
            await new WebHostBuilder()
                .UseUrls("http://*:5000")
                .UseKestrel()
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddSingleton<IWarden>(warden);
                })
                .UseStartup<Elfo.Wardein.APIs.Startup>()
                .Build()
                .RunAsync();
        }

        public async Task ConfigureAndRunAspNetAPIHosting()
        {
            await new WebHostBuilder()
                .UseUrls("http://*:5000")
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddSingleton<IWarden>(warden);
                    serviceCollection.AddScoped<Abstractions.Configuration.IAmWardeinConfigurationManager, OracleWardeinConfigurationManager>();
                })
                .UseStartup<Elfo.Wardein.Backend.Startup>()
                .Build()
                .RunAsync();
        }

        public async Task Stop()
        {
            await warden.StopAsync();
        }
    }
}
