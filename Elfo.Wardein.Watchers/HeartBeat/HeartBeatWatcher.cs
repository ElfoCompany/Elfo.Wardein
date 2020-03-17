using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elfo.Wardein.Integrations.Oracle.Integration;
using Elfo.Wardein.Watchers.HeartBeat.Config;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Warden.Watchers;

namespace Elfo.Wardein.Watchers.HeartBeat
{
    public class HeartBeatWatcher : WardeinWatcher<HeartBeatWatcherConfig>
    {
        private readonly string HeartBeatAppName;


        public HeartBeatWatcher(HeartBeatWatcherConfig config, string name, string group = null) : base(name, config, group )
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Application name can not be empty", nameof(HeartBeatWatcherConfig));

            HeartBeatAppName = name;
        }

        public override async Task<IWatcherCheckResult> ExecuteWatcherActionAsync()
        {
            await UpdateHeartBeatByAppName(Name);
            return await Task.FromResult(Task.CompletedTask as IWatcherCheckResult); 
        }

        public async Task<int> UpdateHeartBeatByAppName(string appName)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration["ConnectionString:Db"];

            OracleIntegrationConfiguration config = new OracleIntegrationConfiguration(connectionString);

            OracleIntegration connection = new OracleIntegration(config);
            // TODO: Check SQL Injection
            var updateDateParameter = new Dictionary<string, object>
            {
                ["DT_LAST_HB"] = new OracleParameter("DT_LAST_HB", OracleDbType.Date).Value = DateTime.UtcNow,
                ["APPL_HOSTNAME"] = new OracleParameter("APPL_HOSTNAME", OracleDbType.Varchar2).Value = appName
            };

            string query = "UPDATE WRD_CNFG SET DT_LAST_HB = :DT_LAST_HB WHERE APPL_HOSTNAME = :APPL_HOSTNAME";

            return await connection.ExecuteAsync(query, updateDateParameter);
        }
    }
}
