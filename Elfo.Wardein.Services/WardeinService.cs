using Elfo.Wardein.Core;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using Microsoft.AspNetCore.Hosting;
using Elfo.Wardein.APIs;
using Elfo.Wardein.Core.Helpers;

namespace Elfo.Wardein.Services
{
    public class WardeinService : MicroService, IMicroService
    {
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
                    Console.WriteLine("Polling at {0}\n", DateTime.Now.ToString("o"));
                    await ServicesContainer.WardeinInstance.RunCheck();
                },
                (e) =>
                {
                    Console.WriteLine("Exception while polling: {0}\n", e.ToString());
                });

                #region Local Functions

                int GetPollingTimeoutInMillisecond() => (int)TimeSpan.FromSeconds(
                    ServicesContainer.WardeinConfigurationReaderService(Const.WARDEIN_CONFIG_PATH)?.GetConfiguration()?.TimeSpanFromSeconds ?? 20
                ).TotalMilliseconds;

                #endregion
            }
        }

        public void Stop()
        {
            this.StopBase();
            Console.WriteLine("I stopped");
        }
    }
}
