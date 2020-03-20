using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Elfo.Firmenich.Wardein.Abstractions.Watchers
{
    public interface IAmWatcherPersistenceService
    {
        Task<WatcherStatusResult> UpsertCurrentStatus(int watcherConfigurationId, int applicationId, int applicationHostname, bool isHealthy, Exception failureException = null);
    }
}
