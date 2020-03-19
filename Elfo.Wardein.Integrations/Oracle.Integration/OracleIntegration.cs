using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Warden;
using Warden.Integrations;
using Warden.Watchers;
using Oracle.ManagedDataAccess.Client;
using Elfo.Wardein.Core.Helpers;

namespace Elfo.Wardein.Integrations.Oracle.Integration
{
    public class OracleIntegration : IIntegration
    {
        private readonly OracleConnectionConfiguration configuration;
        private readonly OracleHelper oracleHelper;

        public OracleIntegration(OracleConnectionConfiguration configuration)
        {
            this.configuration = configuration;
            oracleHelper = new OracleHelper(configuration);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query, IDictionary<string, object> parameters = null)
        {
            try
            {
                return await oracleHelper.QueryAsync<T>(query, parameters);
            } catch (OracleException exception)
            {
                throw new IntegrationException("There was a SQL error while trying to execute the query.", exception);
            } catch (Exception exception)
            {
                throw new IntegrationException("There was an error while trying to access the Oracle database.",
                    exception);
            }
        }

        public async Task<int> ExecuteAsync(string command, IDictionary<string, object> parameters = null)
        {

            try
            {
                return await oracleHelper.ExecuteAsync(command, parameters);
            } catch (OracleException exception)
            {

                throw new IntegrationException("There was a SQL error while trying to execute the command.", exception);
            } catch (Exception exception)
            {
                throw new IntegrationException("There was an error while trying to access the Oracle database.",
                    exception);
            }
        }

        public static OracleIntegration Create(string connectionString,
           Action<OracleConnectionConfiguration.Builder> configurator)
        {
            var config = new OracleConnectionConfiguration.Builder(connectionString);
            configurator?.Invoke(config);

            return Create(config.Build());
        }

        public static OracleIntegration Create(OracleConnectionConfiguration configuration)
            => new OracleIntegration(configuration);

    }
}
