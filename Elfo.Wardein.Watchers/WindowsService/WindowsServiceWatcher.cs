using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Watchers.GenericService;

namespace Elfo.Wardein.Watchers.WindowsService
{
    public class WindowsServiceWatcher : GenericServiceWatcher
    {
        protected WindowsServiceWatcher(GenericServiceConfigurationModel config, string name, string group = null) : base(config, name,  group)
        { }

        public static WindowsServiceWatcher Create(GenericServiceConfigurationModel config, string group = null)
        {
            return new WindowsServiceWatcher(config, $"{nameof(WindowsServiceWatcher)}", group);
        }
    }
}
