using Elfo.Wardein.Services;
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
                var serviceBuilder = new ServiceBuilder();
                var wardeinService = new WardeinMicroService(serviceBuilder);

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

                new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;

                    log.Debug("Starting APIs...");
                    serviceBuilder.ConfigureAndRunAPIHosting().Wait();
                    log.Debug("APIs started");

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
