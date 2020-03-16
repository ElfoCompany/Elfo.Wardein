using System;
using System.Collections.Generic;
using System.Text;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.FileSystem
{
    public class WindowsServiceWatcherCheckResult : WatcherCheckResult
    {
        protected WindowsServiceWatcherCheckResult(FileSystemWatcher watcher, bool isValid, string description) : base(watcher, isValid, description)
        {
        }

        public static WindowsServiceWatcherCheckResult Create(FileSystemWatcher watcher, string description)
        {
            var wasRunSuccessful = string.IsNullOrWhiteSpace(description);
            return new WindowsServiceWatcherCheckResult(watcher, wasRunSuccessful, description);
        }
    }
}
