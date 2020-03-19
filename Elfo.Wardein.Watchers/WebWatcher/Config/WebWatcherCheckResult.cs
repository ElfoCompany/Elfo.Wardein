using System;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.WebWatcher
{
    public class WebWatcherCheckResult :  WatcherCheckResult
    {
        public Uri Uri { get; }

        public IHttpRequest Request { get; }

        public IHttpResponse Response { get; }

        protected WebWatcherCheckResult(WebWatcher watcher, bool isValid, string description,
            Uri uri, IHttpRequest request, IHttpResponse response)
            : base(watcher, isValid, description)
        {
            Uri = uri;
            Request = request;
            Response = response;
        }

        /// <summary>
        /// Factory method for creating a new instance of WebWatcherCheckResult.
        /// </summary>
        /// <param name="watcher">Instance of WebWatcher.</param>
        /// <param name="isValid">Flag determining whether the performed check was valid.</param>
        /// <param name="uri">Base URL of the request.</param>
        /// <param name="request">Instance of IHttpRequest.</param>
        /// <param name="response">Instance of IHttpResponse.</param>
        /// <param name="description">Custom description of the performed check.</param>
        /// <returns>Instance of WebWatcherCheckResult.</returns>
        public static WebWatcherCheckResult Create(WebWatcher watcher, bool isValid, Uri uri,
            IHttpRequest request, IHttpResponse response, string description = "")
            => new WebWatcherCheckResult(watcher, isValid, description, uri, request, response);
    }
}
