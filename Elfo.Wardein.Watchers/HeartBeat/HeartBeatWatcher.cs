using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elfo.Wardein.Oracle;
using Elfo.Wardein.Watchers.HeartBeat.Config;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Warden.Integrations;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.HeartBeat
{
    public class HeartBeatWatcher : WardeinWatcher<HeartBeatWatcherConfig>
    {
        private readonly string HeartBeatAppName;

        public HeartBeatWatcher(HeartBeatWatcherConfig config, string name, string group = null) : base(name, config, group)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Application name can not be empty", nameof(HeartBeatWatcherConfig));

            HeartBeatAppName = name;
        }

        public static HeartBeatWatcher Create(HeartBeatWatcherConfig config, string group = null)
        {
            return new HeartBeatWatcher(config, $"{nameof(HeartBeatWatcher)}", group);
        }

        public override async Task<IWatcherCheckResult> ExecuteWatcherActionAsync()
        {
            var isValid = await UpdateHeartBeatByAppName(HeartBeatAppName);
            return await Task.FromResult(new HeartBeatWatcherCheckResult(this, HeartBeatAppName, true, "Wardein is working"));
        }

        public async Task<bool> UpdateHeartBeatByAppName(string appName)
        {
            //OracleIntegrationConfiguration config = new OracleIntegrationConfiguration(connectionString);

            //OracleIntegration connection = new OracleIntegration(config);
            //// TODO: Check SQL Injection
            var updateDateParameter = new Dictionary<string, object>
            {
                ["DT_LAST_HB"] = new OracleParameter("DT_LAST_HB", OracleDbType.Date).Value = DateTime.UtcNow,
                ["APPL_HOSTNAME"] = new OracleParameter("APPL_HOSTNAME", OracleDbType.Varchar2).Value = appName
            };

            string query = "UPDATE WRD_CNFG SET DT_LAST_HB = :DT_LAST_HB WHERE APPL_HOSTNAME = :APPL_HOSTNAME";

            return await (integration as OracleIntegration).ExecuteAsync(query, updateDateParameter);
        }
    }
}
