using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Elfo.Firmenich.Wardein.Abstractions.Watchers
{
    public interface IAmWatcherPersistenceService
    {
        Task<WatcherStatusResult> UpsertCurrentStatus(int watcherConfigurationId, int applicationId, string applicationHostname, bool isHealthy, Exception failureException = null);
    }
}
