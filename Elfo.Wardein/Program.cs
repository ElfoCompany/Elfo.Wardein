using Elfo.Wardein.APIs;
using Elfo.Wardein.Core;
using Elfo.Wardein.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using PeterKottas.DotNetCore.WindowsService;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.IO;

namespace Elfo.Wardein
{
    class Program
    {
        static Logger log = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            try
            {
                ServiceRunner<WardeinService>.Run(config =>
                {
                    var name = config.GetDefaultName();
                    config.Service(serviceConfig =>
                    {
                        serviceConfig.ServiceFactory((extraArguments, controller) =>
                        {
                            return new WardeinService();
                        });

                        serviceConfig.OnStart((service, extraParams) =>
                        {
                            log.Info("Service {0} started", name);
                            service.Start();

                            ConfigureAPIHosting();

                            #region Local Functions
                            void ConfigureAPIHosting()
                            {
                                new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureServices(serviceCollection =>
                                    {
                                        serviceCollection.AddSingleton<IMicroService>(service);
                                    })
                                    .UseStartup<Startup>()
                                    .Build()
                                    .Run();
                            } 
                            #endregion

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
            }
            catch (Exception ex)
            {
                log.Error("Fatal error in Program: {0}", ex.ToString());
                throw;
            }
        }
    }
}
