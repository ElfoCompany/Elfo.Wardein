using Elfo.Wardein.Abstractions.HeartBeat;
using Elfo.Wardein.Core.Helpers;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elfo.Wardein.Core.HeartBeat
{
    public class OracleWardeinHeartBeatPersistanceService : IAmWardeinHeartBeatPersistanceService
    {
        private readonly OracleConnectionConfiguration oracleConnectionConfiguration;
        private readonly OracleHelper oracleHelper;

        public OracleWardeinHeartBeatPersistanceService(OracleConnectionConfiguration oracleConnectionConfiguration)
        {
            this.oracleConnectionConfiguration = oracleConnectionConfiguration;
            this.oracleHelper = new OracleHelper(oracleConnectionConfiguration);
        }

        public async Task<DateTime> GetLastHearBeat(string applicationHostname)
        {
            var parameters = new Dictionary<string, object>
            {
                ["APPL_HOSTNAME"] = new OracleParameter("APPL_HOSTNAME", OracleDbType.Varchar2).Value = applicationHostname
            };
            var result = await oracleHelper.QueryAsync<DateTime>("select DT_LAST_HB from WRD_CNFG WHERE APPL_HOSTNAME = :APPL_HOSTNAME", parameters);
            return result.FirstOrDefault();
        }

        public async Task<bool> UpdateHeartBeat(string applicationHostname)
        {
            var updateDateParameter = new Dictionary<string, object>
            {
                ["DT_LAST_HB"] = new OracleParameter("DT_LAST_HB", OracleDbType.Date).Value = DateTime.UtcNow,
                ["APPL_HOSTNAME"] = new OracleParameter("APPL_HOSTNAME", OracleDbType.Varchar2).Value = applicationHostname
            };

            string query = "UPDATE WRD_CNFG SET DT_LAST_HB = :DT_LAST_HB WHERE APPL_HOSTNAME = :APPL_HOSTNAME";

            var result = await oracleHelper.ExecuteAsync(query, updateDateParameter);
            return result == 1;
        }
    }
}
