using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Elfo.Wardein.Core.Helpers
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
}
