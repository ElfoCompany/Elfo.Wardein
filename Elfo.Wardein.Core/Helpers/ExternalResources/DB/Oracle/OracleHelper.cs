using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Elfo.Wardein.Core.Helpers
{
    public class OracleHelper
    {
        private readonly OracleConnectionConfiguration oracleConfiguration;

        public OracleHelper(OracleConnectionConfiguration oracleConfiguration)
        {
            this.oracleConfiguration = oracleConfiguration;
        }

        public IEnumerable<T> Query<T>(string query, IDictionary<string, object> parameters = null)
        {
            try
            {
                var result = default(IEnumerable<T>);
                using (var connection = new OracleConnection(oracleConfiguration.ConnectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        connection.SetSessionInfoForTransaction(oracleConfiguration);

                        var queryToExecute = string.IsNullOrWhiteSpace(query) ? oracleConfiguration.Query : query;
                        var queryParameters = parameters ?? oracleConfiguration.QueryParameters;

                        result = oracleConfiguration.OracleServiceProvider().Query<T>(connection, queryToExecute,
                            queryParameters, oracleConfiguration.QueryTimeout);
                        transaction.Commit();
                    }
                }
                return result;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public int Execute(string command, IDictionary<string, object> parameters = null)
        {

            try
            {
                int result = 0;

                using (var connection = new OracleConnection(oracleConfiguration.ConnectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        connection.SetSessionInfoForTransaction(oracleConfiguration);

                        var commandToExecute = string.IsNullOrWhiteSpace(command) ? oracleConfiguration.Command : command;
                        var commandParameters = parameters ?? oracleConfiguration.CommandParameters;

                        result = oracleConfiguration.OracleServiceProvider().Execute(connection, commandToExecute,
                            commandParameters, oracleConfiguration.CommandTimeout);
                        transaction.Commit();
                    }
                }
                return result;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query, IDictionary<string, object> parameters = null)
        {
            try
            {
                var result = default(IEnumerable<T>);
                using (var connection = new OracleConnection(oracleConfiguration.ConnectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        connection.SetSessionInfoForTransaction(oracleConfiguration);

                        var queryToExecute = string.IsNullOrWhiteSpace(query) ? oracleConfiguration.Query : query;
                        var queryParameters = parameters ?? oracleConfiguration.QueryParameters;

                        result = await oracleConfiguration.OracleServiceProvider().QueryAsync<T>(connection, queryToExecute,
                            queryParameters, oracleConfiguration.QueryTimeout);
                        transaction.Commit();
                    }
                }
                return result;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public async Task<int> ExecuteAsync(string command, IDictionary<string, object> parameters = null)
        {

            try
            {
                int result = 0;

                using (var connection = new OracleConnection(oracleConfiguration.ConnectionString))
                {
                    connection.Open();

                    using (var transaction = connection.BeginTransaction())
                    {
                        connection.SetSessionInfoForTransaction(oracleConfiguration);

                        var commandToExecute = string.IsNullOrWhiteSpace(command) ? oracleConfiguration.Command : command;
                        var commandParameters = parameters ?? oracleConfiguration.CommandParameters;

                        result = await oracleConfiguration.OracleServiceProvider().ExecuteAsync(connection, commandToExecute,
                            commandParameters, oracleConfiguration.CommandTimeout);
                        transaction.Commit();
                    }
                }
                return result;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
