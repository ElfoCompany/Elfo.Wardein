using Elfo.Wardein.APIs;
using Elfo.Wardein.Watchers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using PeterKottas.DotNetCore.WindowsService;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.Threading;

namespace Elfo.Wardein
{
    class Program
    {
        static Logger log = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            try
            {
                var wardeinService = new WardeinMicroService();

                //new Thread(() =>
                //{
                //    Thread.CurrentThread.IsBackground = true;

                //    log.Debug("Starting APIs...");
                //    ConfigureAPIHosting();
                //    log.Debug("APIs started");

                //    #region Local Functions
                //    void ConfigureAPIHosting()
                //    {
                //        new WebHostBuilder()
                //            .UseUrls("http://*:5000")
                //            .UseKestrel()
                //            .ConfigureServices(serviceCollection =>
                //            {
                //                serviceCollection.AddSingleton<IMicroService>(wardeinService);
                //            })
                //            .UseStartup<Startup>()
                //            .Build()
                //            .Run();
                //    }
                //    #endregion
                //}).Start();

                new Thread(() =>
                {
                    ServiceRunner<WardeinMicroService>.Run(config =>
                    {
                        var name = config.GetDefaultName();
                        config.Service(serviceConfig =>
                        {
                            serviceConfig.ServiceFactory((extraArguments, controller) =>
                            {
                                return wardeinService;
                            });

                            serviceConfig.OnStart((service, extraParams) =>
                            {
                                log.Info($"Service {name} started");
                                service.Start();
                            });

                            serviceConfig.OnStop(service =>
                            {
                                log.Info($"Service {name} stopped");
                                service.Stop();
                            });

                            serviceConfig.OnError(e =>
                            {
                                log.Error($"Service {name} errored with exception : {e.Message}");
                            });
                        });
                    });
                }).Start();
            }
            catch (Exception ex)
            {
                log.Error($"Fatal error in Program: {ex.ToString()}");
                throw;
            }
        }
    }
}
