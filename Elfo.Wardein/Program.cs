using Elfo.Wardein.APIs;
using Elfo.Wardein.Services;
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
                var wardeinMicroService = new WardeinService();
                var cacheCleanUpService = new CacheCleanUpService();

                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;

                    log.Debug("Starting APIs...");
                    ConfigureAPIHosting();
                    log.Debug("APIs started");

                    #region Local Functions
                    void ConfigureAPIHosting()
                    {
                        new WebHostBuilder()
                            .UseUrls("http://*:5000")
                            .UseKestrel()
                            .ConfigureServices(serviceCollection =>
                            {
                                serviceCollection.AddSingleton<IMicroService>(wardeinMicroService);
                            })
                            .UseStartup<Startup>()
                            .Build()
                            .Run();
                    }
                    #endregion
                }).Start();

                new Thread(() =>
                {
                    ServiceRunner<WardeinService>.Run(config =>
                    {
                        var name = config.GetDefaultName();
                        config.Service(serviceConfig =>
                        {
                            serviceConfig.ServiceFactory((extraArguments, controller) =>
                            {
                                return wardeinMicroService;
                            });

                            serviceConfig.OnStart((service, extraParams) =>
                            {
                                log.Info("Service {0} started", name);
                                service.Start();
                            });

                            serviceConfig.OnStop(service =>
                            {
                                log.Info("Service {0} stopped", name);
                                service.Stop();
                            });

                            serviceConfig.OnError(e =>
                            {
                                log.Error("Service {0} errored with exception : {1}", name, e.Message);
                            });
                        });
                    });
                }).Start();

                new Thread(() =>
                {
                    ServiceRunner<CacheCleanUpService>.Run(config =>
                    {
                        var name = config.GetDefaultName();
                        config.Service(serviceConfig =>
                        {
                            serviceConfig.ServiceFactory((extraArguments, controller) =>
                            {
                                return cacheCleanUpService;
                            });

                            serviceConfig.OnStart((service, extraParams) =>
                            {
                                log.Info("Service {0} started", name);
                                service.Start();
                            });

                            serviceConfig.OnStop(service =>
                            {
                                log.Info("Service {0} stopped", name);
                                service.Stop();
                            });

                            serviceConfig.OnError(e =>
                            {
                                log.Error("Service {0} errored with exception : {1}", name, e.Message);
                            });
                        });
                    });
                }).Start();
            }
            catch (Exception ex)
            {
                log.Error("Fatal error in Program: {0}", ex.ToString());
                throw;
            }
        }
    }
}
