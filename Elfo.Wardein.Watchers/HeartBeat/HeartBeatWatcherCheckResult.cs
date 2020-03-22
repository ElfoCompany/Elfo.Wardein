using System;
using System.Collections.Generic;
using System.Text;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.HeartBeat
{
    class HeartBeatWatcherCheckResult : WatcherCheckResult
    {
        public string HeartBeatAppName { get; }

        public HeartBeatWatcherCheckResult(HeartBeatWatcher watcher, string heartBeatAppName, bool isValid, string description)
        : base(watcher, isValid, description)
        {
            HeartBeatAppName = heartBeatAppName;
        }
    }
}
