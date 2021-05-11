using System;
using System.Collections.Generic;
using System.Text;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.PerformanceWatcher
{
    public class PerformanceWatcherCheckResult : WatcherCheckResult
    {
        public PerformanceWatcherCheckResult(PerformanceWatcher watcher, bool isValid, string description, Uri uri) : base(watcher, isValid, description)
        {
            Uri = uri;
        }

        public Uri Uri { get; }


        /// <summary>
        /// Factory method for creating a new instance of PerformanceWatcherCheckResult.
        /// </summary>
        /// <param name="watcher">Instance of PerformanceWatcher.</param>
        /// <param name="isValid">Flag determining whether the performed check was valid.</param>
        /// <param name="uri">Base URL of the request.</param>
        /// <param name="request">Instance of IHttpRequest.</param>
        /// <param name="response">Instance of IHttpResponse.</param>
        /// <param name="description">Custom description of the performed check.</param>
        /// <returns>Instance of PerformanceWatcherCheckResult.</returns>
        public static PerformanceWatcherCheckResult Create(PerformanceWatcher watcher, bool isValid, Uri uri, string description = "")
        {
            return new PerformanceWatcherCheckResult(watcher, isValid, description, uri);
        }
    }
}
