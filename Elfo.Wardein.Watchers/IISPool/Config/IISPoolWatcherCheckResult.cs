using System;
using System.Collections.Generic;
using System.Text;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.IISPool
{
    public class IISPoolWatcherCheckResult : WatcherCheckResult
    {

        protected IISPoolWatcherCheckResult(IISPoolWatcher watcher, bool isValid, string description) : base(watcher, isValid, description)
        {
        }

        public static IISPoolWatcherCheckResult Create(IISPoolWatcher watcher, string description)
        {
            var wasRunSuccessful = string.IsNullOrWhiteSpace(description);
            return new IISPoolWatcherCheckResult(watcher, wasRunSuccessful, description);
        }
    }
}
