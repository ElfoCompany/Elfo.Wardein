using Elfo.Firmenich.Wardein.Abstractions.Watchers;
using Elfo.Wardein.Core.Helpers;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elfo.Firmenich.Wardein.Core.Persistence
{
    public class OracleWatcherPersistenceService : IAmWatcherPersistenceService
    {
        private readonly OracleConnectionConfiguration oracleConnectionConfiguration;
        private readonly OracleHelper oracleHelper;

        public OracleWatcherPersistenceService(OracleConnectionConfiguration oracleConnectionConfiguration)
        {
            this.oracleConnectionConfiguration = oracleConnectionConfiguration;
            this.oracleHelper = new OracleHelper(oracleConnectionConfiguration);
        }

        public async Task<WatcherStatusResult> UpsertCurrentStatus(int watcherConfigurationId, int applicationId, string applicationHostname, bool isHealthy, Exception failureException = null)
        {
            return await this.oracleHelper.CallProcedureAsync<WatcherStatusResult>(
                packageName: "PKG_WRD",
                procedureName: "PRC_UPS_WTCH_RES",
                howToGetResult: (parameters) =>
                {
                    int.TryParse(parameters.FirstOrDefault(x => x.ParameterName == "po_flr_count")?.Value?.ToString(), out int errorCount);
                    bool wasHealthy = parameters.FirstOrDefault(x => x.ParameterName == "po_prv_status")?.Value?.ToString()?.ToUpperInvariant() == "Y";
                    return new WatcherStatusResult()
                    {
                        FailureCount = errorCount,
                        PreviousStatus = wasHealthy
                    };
                },
                new OracleParameter("p_wtchr_cnfg_id", OracleDbType.Int32, watcherConfigurationId, System.Data.ParameterDirection.Input),
                new OracleParameter("p_appl_id", OracleDbType.Int32, applicationId, System.Data.ParameterDirection.Input),
                new OracleParameter("p_hostname", OracleDbType.Varchar2, applicationId, System.Data.ParameterDirection.Input),
                new OracleParameter("p_is_healthy", OracleDbType.Varchar2, applicationId, System.Data.ParameterDirection.Input),
                new OracleParameter("p_flr_msg", OracleDbType.Varchar2, applicationId, System.Data.ParameterDirection.Input),
                new OracleParameter("po_flr_count", OracleDbType.Int32, applicationId, System.Data.ParameterDirection.Output),
                new OracleParameter("po_prv_status", OracleDbType.Varchar2, applicationId, System.Data.ParameterDirection.Output)
            );
        }
    }
}
