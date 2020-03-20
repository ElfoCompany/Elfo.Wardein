using Elfo.Firmenich.Wardein.Abstractions.Watchers;
using Elfo.Wardein.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Elfo.Firmenich.Wardein.Core.Persistence
{
    public class OracleWatcherPersistenceService : IAmWatcherPersistenceService
    {
        private readonly OracleConnectionConfiguration oracleConnectionConfiguration;

        public OracleWatcherPersistenceService(OracleConnectionConfiguration oracleConnectionConfiguration)
        {
            this.oracleConnectionConfiguration = oracleConnectionConfiguration;
        }

        public Task<WatcherStatusResult> UpsertCurrentStatus(int watcherConfigurationId, int applicationId, int applicationHostname, bool isHealthy, Exception failureException = null)
        {
            throw new NotImplementedException();
        }
    }
}
