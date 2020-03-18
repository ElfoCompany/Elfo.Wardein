using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Elfo.Wardein.Oracle
{
    public interface IOracleService
    {
        IEnumerable<T> Query<T>(IDbConnection connection, string query,
          IDictionary<string, object> parameters, TimeSpan? timeout = null);

        int Execute(IDbConnection connection, string command,
            IDictionary<string, object> parameters, TimeSpan? timeout = null);

        Task<IEnumerable<T>> QueryAsync<T>(IDbConnection connection, string query,
          IDictionary<string, object> parameters, TimeSpan? timeout = null);

        Task<int> ExecuteAsync(IDbConnection connection, string command,
            IDictionary<string, object> parameters, TimeSpan? timeout = null);
    }

    public class DapperOracleService : IOracleService
    {
        public IEnumerable<T> Query<T>(IDbConnection connection, string query,
            IDictionary<string, object> parameters, TimeSpan? timeout = null)
        {
            var queryParameters = new DynamicParameters();
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    queryParameters.Add(parameter.Key, parameter.Value);
                }
            }

            return connection.Query<T>(query, queryParameters,
                commandTimeout: (int?)timeout?.TotalSeconds);
        }

        public int Execute(IDbConnection connection, string command,
           IDictionary<string, object> parameters, TimeSpan? timeout = null)
        {
            var commandParameters = new DynamicParameters();
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    commandParameters.Add(parameter.Key, parameter.Value);
                }
            }

            return connection.Execute(command, parameters,
                commandTimeout: (int?)timeout?.TotalSeconds);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(IDbConnection connection, string query,
                IDictionary<string, object> parameters, TimeSpan? timeout = null)
        {
            var queryParameters = new DynamicParameters();
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    queryParameters.Add(parameter.Key, parameter.Value);
                }
            }

            return await connection.QueryAsync<T>(query, queryParameters,
                commandTimeout: (int?)timeout?.TotalSeconds);
        }

        public async Task<int> ExecuteAsync(IDbConnection connection, string command,
           IDictionary<string, object> parameters, TimeSpan? timeout = null)
        {
            var commandParameters = new DynamicParameters();
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    commandParameters.Add(parameter.Key, parameter.Value);
                }
            }

            return await connection.ExecuteAsync(command, parameters,
                commandTimeout: (int?)timeout?.TotalSeconds);
        }
    }
}