using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Watchers.GenericService;


namespace Elfo.Wardein.Watchers.IISPool
{
    public class IISPoolWatcher : GenericServiceWatcher
    {
        protected IISPoolWatcher(GenericServiceConfigurationModel config, string name, string group = null) : base(config, name, group)
        { }

        public static IISPoolWatcher Create(GenericServiceConfigurationModel config, string group = null)
        {
            return new IISPoolWatcher(config, $"{nameof(IISPoolWatcher)}", group);
        }

    }
}
