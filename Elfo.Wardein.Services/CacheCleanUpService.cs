﻿using Elfo.CleanUpManager;
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
                            log.Debug("Polling at {0} started", DateTime.Now.ToString("o"));

                            CleanUpOptions cleanUpOptions = new CleanUpOptions(cleanUp.FilePath + "*.*");
                            cleanUpOptions.Recursive = true;
                            cleanUpOptions.RemoveEmptyFolders = true;
                            cleanUpOptions.Seconds = cleanUp.CleanUpOptions.ThresholdInSeconds;
                            cleanUpOptions.Days = cleanUp.CleanUpOptions.ThresholdInDays;

                            Cleaner filesProcessor = new Cleaner(log, cleanUpOptions);
                            filesProcessor.CleanUp();

                            //Activity
                            log.Debug("Polling at {0} finished", DateTime.Now.ToString("o"));
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