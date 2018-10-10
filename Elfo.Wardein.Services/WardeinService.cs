using Elfo.Wardein.Core;
using Elfo.Wardein.Core.Helpers;
using NLog;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;

namespace Elfo.Wardein.Services
{
    public class WardeinService : MicroService, IMicroService
    {
        static Logger log = LogManager.GetCurrentClassLogger();
        public void Start()
        {
            log.Info("---\tInitializing WardeinService\t---");
            ConfigureScheduledServiceCheck();

            void ConfigureScheduledServiceCheck()
            {
                log.Info("---\tStarting WardeinService\t---");
                this.StartBase();
                // TODO: Read polling timeout from config
                var x = GetPollingTimeoutInMillisecond();
                Timers.Start("Poller", x, async () =>
                {
                    try
                    {
                        log.Info($"{Environment.NewLine}------------------------------- Polling at {DateTime.Now.ToString("o")} started -------------------------------");
                        await ServicesContainer.WardeinInstance.RunCheck();
                        log.Info($"{Environment.NewLine}------------------------------- Polling at {DateTime.Now.ToString("o")} finished -------------------------------{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}");
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, $"Exception inside polling action: {ex.ToString()}\n");
                    }
                },
                (e) =>
                {
                    log.Error(e, "Exception while polling");
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
            log.Info("---\tWardein Service stopped\t---");
        }
    }
}
