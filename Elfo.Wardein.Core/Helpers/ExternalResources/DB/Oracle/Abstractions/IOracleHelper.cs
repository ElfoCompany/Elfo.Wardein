using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elfo.Wardein.Core.Helpers
{
    public interface IOracleHelper
    {
        Task<T> CallProcedureAsync<T>(string packageName, string procedureName, Func<OracleParameter[], T> howToGetResult, params OracleParameter[] parameters);
        int Execute(string command, IDictionary<string, object> parameters = null);
        Task<int> ExecuteAsync(string command, IDictionary<string, object> parameters = null);
        IEnumerable<T> Query<T>(string query, IDictionary<string, object> parameters = null);
        Task<IEnumerable<T>> QueryAsync<T>(string query, IDictionary<string, object> parameters = null);
    }
}