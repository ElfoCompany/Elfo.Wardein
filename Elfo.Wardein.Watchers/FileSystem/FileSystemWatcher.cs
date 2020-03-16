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
using System.Threading.Tasks;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.FileSystem
{
    public class FileSystemWatcher : WardeinWatcher<FileSystemWatcherConfig>
    {
        protected FileSystemWatcher(FileSystemWatcherConfig config, string group = null) : base(nameof(FileSystemWatcher), config, group)
        { }

        public static FileSystemWatcher Create(FileSystemWatcherConfig config, string group = null)
        {
            return new FileSystemWatcher(config, group);
        }

        public override async Task<IWatcherCheckResult> ExecuteWatcherActionAsync()
        {
            log.Info("---\tStarting FileSystemWatcher\t---");

            var resultDescription = new StringBuilder(string.Empty);

            foreach (var cleanUp in Config.CleanUps)
            {
                var iterationMessage = string.Empty;
                try
                {
                    var guid = Guid.NewGuid();
                    log.Info($"{Environment.NewLine}{"-".Repeat(24)} Cache cleanup @ {guid} started {"-".Repeat(24)}");

                    var options = GetCleanUpOptions(cleanUp);
                    var filesProcessor = new Cleaner(log, options);

                    filesProcessor.CleanUp();

                    log.Info($"{Environment.NewLine}{"-".Repeat(24)} Cache cleanup @ {guid} finished {"-".Repeat(24)}{Environment.NewLine.Repeat(3)}");
                }
                catch (Exception ex)
                {
                    iterationMessage = $"Exception inside path: {cleanUp.FilePath}: {ex.ToString()} stack trace: {ex.StackTrace}{Environment.NewLine}";
                    resultDescription.AppendLine(iterationMessage);
                    log.Error(ex, iterationMessage);
                }
            }

            var watcherResultDescription = resultDescription.ToString();
            return await Task.FromResult(WindowsServiceWatcherCheckResult.Create(this, watcherResultDescription));
        }

        private CleanUpOptions GetCleanUpOptions(FileSystemCleanUpConfig cleanUp)
        {
            var options = new CleanUpOptions(cleanUp.FilePath);
            options.RemoveEmptyFolders = cleanUp.CleanUpOptions.RemoveEmptyFolders;
            options.DisplayOnly = cleanUp.CleanUpOptions.DisplayOnly;
            options.RemoveEmptyFolders = cleanUp.CleanUpOptions.RemoveEmptyFolders;
            options.UseRecycleBin = cleanUp.CleanUpOptions.UseRecycleBin;
            options.Recursive = cleanUp.CleanUpOptions.Recursive;
            ConfigureThreshold();
            return options;

            #region Local Functions
            void ConfigureThreshold()
            {
                if (cleanUp.CleanUpOptions.ThresholdInSeconds != default(int) && cleanUp.CleanUpOptions.ThresholdInDays == default(int))
                    options.Seconds = cleanUp.CleanUpOptions.ThresholdInSeconds;
                else if (cleanUp.CleanUpOptions.ThresholdInSeconds == default(int) && cleanUp.CleanUpOptions.ThresholdInDays != default(int))
                    options.Days = cleanUp.CleanUpOptions.ThresholdInDays;
                else if (cleanUp.CleanUpOptions.ThresholdInSeconds != default(int) && cleanUp.CleanUpOptions.ThresholdInDays != default(int))
                    options.Days = cleanUp.CleanUpOptions.ThresholdInDays;
                else
                    options.Seconds = 300;
            }
            #endregion
        }
    }
}
