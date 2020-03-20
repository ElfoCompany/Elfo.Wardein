using Elfo.Wardein.Abstractions.Configuration;
using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Core.Helpers;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elfo.Firmenich.Wardein.Core.ConfigurationManagers
{
    public class OracleWardeinConfigurationManager : IAmWardeinConfigurationManager
    {
        private readonly OracleConnectionConfiguration configuration;
        private readonly string hostname;
        private readonly OracleHelper oracleHelper;

        public OracleWardeinConfigurationManager(OracleConnectionConfiguration configuration, string hostname)
        {
            this.configuration = configuration;
            this.hostname = hostname;
            this.oracleHelper = new OracleHelper(configuration);
        }

        public bool IsInMaintenanceMode => throw new NotImplementedException();

        public WardeinConfig GetConfiguration()
        {
            // TODO: Add dynamic Appl hostname and check if it's possible ot remove deplendency towards managed data access
            var parameters = new Dictionary<string, object>
            {
                ["APPL_HOSTNAME"] = new OracleParameter("APPL_HOSTNAME", OracleDbType.Varchar2).Value = hostname
            };
            var query = @"SELECT * FROM V_WRD_WATCHERS WHERE ""ApplicationHostname"" = :APPL_HOSTNAME";
            var result = this.oracleHelper.Query<WardeinConfigurationModel>(query, parameters);

            // TODO: Test to see if it works.. code will probably need refactoring
            var json = JObject.Parse(result.FirstOrDefault().WardeinConfig);
            foreach (var watcherConfig in result)
            {
                var watcherTypeConfigTest = JObject.Parse(watcherConfig.WatcherTypeJsonConfig);
                var watcherConfigTest = JObject.Parse(watcherConfig.WatcherJsonConfig);
                watcherTypeConfigTest.Merge(watcherConfigTest);
                json.Merge(watcherTypeConfigTest);
            }

            // TODO: Cahe the config

            return json.ToObject<WardeinConfig>();
        }

        public void InvalidateCache()
        {
            throw new NotImplementedException();
        }

        public void StartMaintenanceMode(double durationInSeconds)
        {
            throw new NotImplementedException();
        }

        public void StopMaintenaceMode()
        {
            throw new NotImplementedException();
        }
    }

    public class WardeinConfigurationModel
    {
        public int WatcherConfigurationId { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationHostname { get; set; }
        public string WatcherJsonConfig { get; set; }
        public string WatcherTypeJsonConfig { get; set; }
        public string WardeinConfig { get; set; }
    }
}
