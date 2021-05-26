using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Configurations;
using Elfo.Wardein.Core;
using Elfo.Wardein.Core.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Warden;

namespace Elfo.Wardein.Services
{
    public class ServiceBuilder
    {
        static IWarden warden;
        protected static ILogger log = LogManager.GetCurrentClassLogger();

        public async Task ConfigureAndRunWarden()
        {
            var wardeinConfigurationManager = ServicesContainer.WardeinConfigurationManager();
            WardeinConfig wardeinConfiguration = new WardeinConfig();
            //using(var client = new HttpClient())
            //{
            //try
            //{
            //    var result = await client.GetStringAsync($"http://localhost:{ServicesContainer.WardeinBaseConfiguration().BackendBasePort}/api/new/configs");
            //    wardeinConfiguration = JsonConvert.DeserializeObject<WardeinConfig>(result);
            //}
            //catch (Exception)
            //{
            wardeinConfiguration = wardeinConfigurationManager.GetConfiguration();
            //}
            //}

            var configurationBuilder = WardenHelper.GetWardenConfigurationBuilder(wardeinConfiguration);
            configurationBuilder
                .SetHooks(hooks =>
                {
                    hooks.OnStart(() => ConfigureAndRunAspNetAPIHosting());
                    hooks.OnIterationStart(n => log.Debug($"Iteration n°{n} started{"-".Repeat(5)}{Environment.NewLine.Repeat(2)}"));
                    hooks.OnIterationCompleted(iteration => log.Debug($"Iteration completed{"-".Repeat(5)}{Environment.NewLine.Repeat(2)}"));
                });

            warden = WardenInstance.Create(configurationBuilder.Build());
            await warden.StartAsync();
        }

        public async Task ConfigureAndRunAspNetAPIHosting()
        {
            new Thread(() =>
            {
                try
                {
                    var path = Path.Combine(Path.GetTempPath(), "Wardein");
                    var fileName = "cache.invalidate";
                    log.Debug($"Monitoring {path}");
                    System.IO.Directory.CreateDirectory(path);
                    var watcher = new System.IO.FileSystemWatcher(path, fileName);
                    watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                 | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                    watcher.Created += (e, args) =>
                    {
                        log.Debug($"Something changed in {path}");
                        System.IO.File.Delete(Path.Combine(path, fileName));
                        warden.StopAsync();
                        throw new RestartWardeinServiceWithFakeException();
                    };
                    watcher.EnableRaisingEvents = true;
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                }
            }).Start();

            log.Debug("Initializing APIs");
            await WebHost.CreateDefaultBuilder()
                .UseUrls($"http://*:{ServicesContainer.WardeinBaseConfiguration().BackendBasePort}")
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddSingleton<IWarden>(warden);
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


    public class RestartWardeinServiceWithFakeException : Exception
    {

    }
}
