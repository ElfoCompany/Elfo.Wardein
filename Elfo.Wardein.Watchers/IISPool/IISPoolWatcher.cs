using Elfo.Wardein.Watchers.GenericService;


namespace Elfo.Wardein.Watchers.IISPool
{
    public class IISPoolWatcher : GenericServiceWatcher
    {
        protected IISPoolWatcher(GenericServiceWatcherConfig config, string name, string group = null) : base(config, name, group)
        { }

        public static IISPoolWatcher Create(GenericServiceWatcherConfig config, string group = null)
        {
            return new IISPoolWatcher(config, $"{nameof(IISPoolWatcher)}", group);
        }

    }
}
