using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace Elfo.Wardein.Oracle
{
    public class OracleConnectionConfiguration
    {
        public OracleConnectionConfiguration(string connectionString)
        {
            ConnectionString = connectionString;

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException("Oracle connection string can not be empty.", nameof(connectionString));

            try
            {
                var oracleConnectionStringBuilder = new OracleConnectionStringBuilder(connectionString);
                DatabaseType = oracleConnectionStringBuilder.DataSource;
            } catch (Exception ex)
            {
                throw new ArgumentException("Oracle connection string is invalid.", nameof(connectionString), ex);
            }

            ConnectionString = connectionString;
            ConnectionProvider = oracleConnectionString => new OracleConnection(oracleConnectionString);
            OracleServiceProvider = () => new DapperOracleService();
        }


        public string ConnectionString { get; protected set; }
        public string DatabaseType { get; protected set; }
        public string Query { get; protected set; }
        public IDictionary<string, object> QueryParameters { get; protected set; }
        public TimeSpan? QueryTimeout { get; protected set; }
        public Func<string, IDbConnection> ConnectionProvider { get; protected set; }
        public Func<IOracleService> OracleServiceProvider { get; protected set; }
        public string Command { get; protected set; }
        public IDictionary<string, object> CommandParameters { get; protected set; }
        public TimeSpan? CommandTimeout { get; protected set; }

        public string ClientId { get; set; }
        public string ClientInfo { get; set; }
        public string ModuleName { get; set; }
        public string DateLanguage { get; set; }


        public static Builder Create(string connectionString) => new Builder(connectionString);

        public class Builder
        {
            protected readonly OracleConnectionConfiguration Configuration;

            public Builder(string connectionString)
            {
                Configuration = new OracleConnectionConfiguration(connectionString);
            }

            public Builder WithCommand(string command, IDictionary<string, object> parameters = null)
            {
                if (string.IsNullOrEmpty(command))
                    throw new ArgumentException("SQL command can not be empty.", nameof(command));

                Configuration.Command = command;
                Configuration.CommandParameters = parameters;

                return this;
            }

            public Builder WithCommandTimeout(TimeSpan timeout)
            {
                if (timeout == null)
                    throw new ArgumentNullException(nameof(timeout), "SQL command timeout can not be null.");

                if (timeout == TimeSpan.Zero)
                    throw new ArgumentException("SQL command timeout can not be equal to zero.", nameof(timeout));

                Configuration.CommandTimeout = timeout;

                return this;
            }

            public Builder WithQuery(string query, IDictionary<string, object> parameters = null)
            {
                if (string.IsNullOrEmpty(query))
                    throw new ArgumentException("SQL query can not be empty.", nameof(query));

                Configuration.Query = query;
                Configuration.QueryParameters = parameters;

                return this;
            }

            public Builder WithQueryTimeout(TimeSpan timeout)
            {
                if (timeout == null)
                    throw new ArgumentNullException(nameof(timeout), "SQL query timeout can not be null.");

                if (timeout == TimeSpan.Zero)
                    throw new ArgumentException("SQL query timeout can not be equal to zero.", nameof(timeout));

                Configuration.QueryTimeout = timeout;

                return this;
            }

            public Builder WithConnectionProvider(Func<string, IDbConnection> connectionProvider)
            {
                if (connectionProvider == null)
                {
                    throw new ArgumentNullException(nameof(connectionProvider),
                        "SQL connection provider can not be null.");
                }

                Configuration.ConnectionProvider = connectionProvider;

                return this;
            }

            public Builder WithOracleServiceProvider(Func<IOracleService> oracleServiceProvider)
            {
                if (oracleServiceProvider == null)
                {
                    throw new ArgumentNullException(nameof(oracleServiceProvider),
                        "Oracle service provider can not be null.");
                }

                Configuration.OracleServiceProvider = oracleServiceProvider;

                return this;
            }

            public Builder WithClientId(string clientId)
            {
                if (string.IsNullOrWhiteSpace(clientId))
                {
                    throw new ArgumentNullException(nameof(clientId),
                        "ClientId can not be empty");
                }

                Configuration.ClientId = clientId;

                return this;
            }

            public Builder WithClientInfo(string clientInfo)
            {
                if (string.IsNullOrWhiteSpace(clientInfo))
                {
                    throw new ArgumentNullException(nameof(clientInfo),
                        "ClientInfo can not be empty.");
                }

                Configuration.ClientId = clientInfo;

                return this;
            }

            public Builder WithModuleName(string moduleName)
            {
                if (string.IsNullOrWhiteSpace(moduleName))
                {
                    throw new ArgumentNullException(nameof(moduleName),
                        "ModuleName can not be empty.");
                }

                Configuration.ClientId = moduleName;

                return this;
            }

            public Builder WithDateLanguage(string dateLanguage)
            {
                if (string.IsNullOrWhiteSpace(dateLanguage))
                {
                    throw new ArgumentNullException(nameof(dateLanguage),
                        "DateLanguage can not be empty.");
                }

                Configuration.ClientId = dateLanguage;

                return this;
            }

            public OracleConnectionConfiguration Build() => Configuration;
        }
    }
}
