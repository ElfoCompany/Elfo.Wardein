using Elfo.CleanUpManager;
using Elfo.Wardein.Core;
using Elfo.Wardein.Core.Helpers;
using Elfo.Wardein.Core.Models;
using NLog;
using PeterKottas.DotNetCore.WindowsService.Base;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Services
{
    public class CacheCleanUpService : MicroService, IMicroService
    {
        static Logger log = LogManager.GetCurrentClassLogger();

        public void Start()

        {
            log.Info("---\tInitializing CacheCleanUpService\t---");
            ConfigureScheduledServiceCheck();

            void ConfigureScheduledServiceCheck()
            {
                log.Info("---\tStarting CacheCleanUpService\t---");
                this.StartBase();
                // TODO: Read polling timeout from config
                var cleanUps = GetCleanUps();
                foreach (var cleanUp in cleanUps)
                {
                    Timers.Start("Poller for " + cleanUp.FilePath, (int)TimeSpan.FromSeconds(cleanUp.TimeSpanFromSeconds).TotalMilliseconds, async () =>
                    {
                        try
                        {
                            var guid = Guid.NewGuid();
                            log.Info($"{Environment.NewLine}--------------------------- Cache cleanup @ {guid} started ---------------------------");

                            CleanUpOptions cleanUpOptions = new CleanUpOptions(cleanUp.FilePath);
                            cleanUpOptions.RemoveEmptyFolders = true;
                            cleanUpOptions.Seconds = cleanUp.CleanUpOptions.ThresholdInSeconds;
                            cleanUpOptions.Days = cleanUp.CleanUpOptions.ThresholdInDays;
                            cleanUpOptions.DisplayOnly = cleanUp.CleanUpOptions.DisplayOnly;
                            cleanUpOptions.RemoveEmptyFolders = cleanUp.CleanUpOptions.RemoveEmptyFolders;
                            cleanUpOptions.UseRecycleBin = cleanUp.CleanUpOptions.UseRecycleBin;
                            cleanUpOptions.Recursive = cleanUp.CleanUpOptions.Recursive;

                            Cleaner filesProcessor = new Cleaner(log, cleanUpOptions);
                            filesProcessor.CleanUp();

                            //Activity
                            log.Info($"{Environment.NewLine}--------------------------- Cache cleanup @ {guid} finished ---------------------------{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}");
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
                }
                #region Local Functions

                CleanUps[] GetCleanUps() => (CleanUps[])
                    ServicesContainer.WardeinConfigurationManager(Const.WARDEIN_CONFIG_PATH)?.GetConfiguration()?.CleanUps;
                #endregion
            }
        }

        public void Stop()
        {
            this.StopBase();
            log.Info("---\tCacheCleanUpService Service stopped\t---");
        }
    }
}
