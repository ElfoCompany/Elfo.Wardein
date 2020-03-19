using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Core.Helpers
{
    public static class OracleConnectionExtensionMethods
    {
        public static void SetSessionInfoForTransaction(this OracleConnection connection, OracleConnectionConfiguration oracleConfiguration)
        {
            connection.ClientId = oracleConfiguration.ClientId;
            connection.ClientInfo = oracleConfiguration.ClientInfo;
            connection.ModuleName = oracleConfiguration.ModuleName;
            var sessionInfo = connection.GetSessionInfo();
            sessionInfo.DateLanguage = oracleConfiguration.DateLanguage;
            connection.SetSessionInfo(sessionInfo);
        }
    }
}
