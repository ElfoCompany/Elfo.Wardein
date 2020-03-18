using Elfo.Wardein.Watchers.GenericService;

namespace Elfo.Wardein.Watchers.WindowsService
{
    public class WindowsServiceWatcher : GenericServiceWatcher
    {
        protected WindowsServiceWatcher(GenericServiceWatcherConfig config, string name, string group = null) : base(config, name,  group)
        { }

        public static WindowsServiceWatcher Create(GenericServiceWatcherConfig config, string group = null)
        {
            return new WindowsServiceWatcher(config, $"{nameof(WindowsServiceWatcher)}", group);
        }
    }
}
