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

namespace Elfo.Wardein.Services
{
    public class ServiceBuilder
    {
        static IWarden warden;

        public async Task ConfigureAndRunWarden()
        {
            var appConfiguration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var connectionString = appConfiguration["ConnectionStrings:Db"];

            var configuration = WardenConfiguration
                .Create()
                .IntegrateWithOracle(connectionString)
                .AddFileSystemWatcher(null)
                .AddWindowsServiceWatcher(null)
                .AddIISPoolWatcher(null)
                .Build();

            warden = WardenInstance.Create(configuration);
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
