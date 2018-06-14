using Elfo.Wardein.Core;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using Microsoft.AspNetCore.Hosting;
using Elfo.Wardein.APIs;
using Elfo.Wardein.Core.Helpers;
using NLog;

namespace Elfo.Wardein.Services
{
    public class WardeinService : MicroService, IMicroService
    {
        static Logger log = LogManager.GetCurrentClassLogger();
        public void Start()
        {
            ConfigureScheduledServiceCheck();
            ConfigureAPIHosting();

            void ConfigureAPIHosting()
            {
                new WebHostBuilder()
                    .UseKestrel()
                    .UseStartup<Startup>()
                    .Build()
                    .Run();
            }

            void ConfigureScheduledServiceCheck()
            {
                this.StartBase();
                // TODO: Read polling timeout from config
                Timers.Start("Poller", GetPollingTimeoutInMillisecond(), async () =>
                {
                    try
                    {
                        log.Info("Polling at {0} started", DateTime.Now.ToString("o"));
                        await ServicesContainer.WardeinInstance.RunCheck();
                        log.Info("Polling at {0} finished", DateTime.Now.ToString("o"));
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex,$"Exception inside polling action: {ex.ToString()}\n");
                    }
                },
                (e) =>
                {
                    log.Error(e,"Exception while polling");
                });

                #region Local Functions

                int GetPollingTimeoutInMillisecond() => (int)TimeSpan.FromSeconds(
                    ServicesContainer.WardeinConfigurationManager(Const.WARDEIN_CONFIG_PATH)?.GetConfiguration()?.TimeSpanFromSeconds ?? 20
                ).TotalMilliseconds;

                #endregion
            }
        }

        public void Stop()
        {
            this.StopBase();
            log.Info("I stopped");
        }
    }
}
