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

namespace Elfo.Wardein.Services
{
    public class ServiceBuilder
    {
        static IWarden warden;

        public async Task ConfigureAndRunWarden()
        {
            var wardeinConfigurationManager = ServicesContainer.WardeinConfigurationManager();
            var wardeinConfiguration = wardeinConfigurationManager.GetConfiguration();

            var configurationBuilder = WardenConfiguration
                .Create()
                .SetHooks(hooks => hooks.OnStart(() =>  ConfigureAndRunAPIHosting()))
                .AddWardeinHeartBeatWatcher(wardeinConfiguration.Heartbeat, "HeartbeatWatcher");

            // TODO: refactor and add oracle integration (even if useles at the moment)

            if (wardeinConfiguration.Urls?.Count() > 0)
                foreach (var url in wardeinConfiguration.Urls)
                    configurationBuilder.AddWebWatcher(url, "WebWatcher");

            if (wardeinConfiguration.Services?.Count() > 0)
                foreach (var service in wardeinConfiguration.Services)
                    configurationBuilder.AddWindowsServiceWatcher(service, "ServiceWatcher");

            if (wardeinConfiguration.IISPools?.Count() > 0)
                foreach (var pool in wardeinConfiguration.IISPools)
                    configurationBuilder.AddIISPoolWatcher(pool, "WebWatcher");

            if (wardeinConfiguration.CleanUps?.Count() > 0)
                foreach (var cleanUp in wardeinConfiguration.CleanUps)
                    configurationBuilder.AddFileSystemWatcher(cleanUp, "CleanUpWatcher");

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
                .UseStartup<Startup>()
                .Build()
                .RunAsync();
        }
    }
}
