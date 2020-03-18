using Elfo.Wardein.Abstractions.Configuration;
using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Oracle;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elfo.Wardein.Core.ConfigurationManagers
{
    public class WardeinConfigurationManagerFromOracle : IAmWardeinConfigurationManager
    {
        private readonly OracleConnectionConfiguration configuration;
        private readonly OracleRepository oracleRepository;

        public WardeinConfigurationManagerFromOracle(OracleConnectionConfiguration configuration)
        {
            this.configuration = configuration;
            this.oracleRepository = new OracleRepository(configuration);
        }

        public bool IsInMaintenanceMode => throw new NotImplementedException();

        public WardeinConfig GetConfiguration()
        {
            // TODO: Add dynamic Appl hostname and check if it's possible ot remove deplendency towards managed data access
            var parameters = new Dictionary<string, object>
            {
                ["APPL_HOSTNAME"] = new OracleParameter("APPL_HOSTNAME", OracleDbType.Varchar2).Value = "SRVWEB07"
            };
            var query = @"SELECT * FROM V_WRD_WATCHERS WHERE ""ApplicationHostname"" = :APPL_HOSTNAME";
            var result = this.oracleRepository.Query<WatcherModel>(query, parameters);
            
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

    public class WatcherModel
    {
        public int WatcherConfigurationId { get; set; }
        public int ApplicationId { get; set; }
        public string ApplicationHostname { get; set; }
        public string WatcherJsonConfig { get; set; }
        public string WatcherTypeJsonConfig { get; set; }
        public string WardeinConfig { get; set; }
    }
}
