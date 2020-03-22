using Warden.Watchers;

namespace Elfo.Wardein.Watchers.GenericService
{
    public class GenericServiceWatcherCheckResult : WatcherCheckResult
    {
        protected GenericServiceWatcherCheckResult(GenericServiceWatcher watcher, bool isValid, string description) : base(watcher, isValid, description)
        {
        }

        public static GenericServiceWatcherCheckResult Create(GenericServiceWatcher watcher, string description)
        {
            var wasRunSuccessful = string.IsNullOrWhiteSpace(description);
            return new GenericServiceWatcherCheckResult(watcher, wasRunSuccessful, description);
        }
    }
}
