using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Core
{
    public class WardeinBaseConfiguration
    {
        public ConnectionType StorageConnectionType { get; set; }
        public string StorageConnectionString { get; set; }
        public OracleSessionParameters OracleAdditionalParams { get; set; }

        public ConnectionType MailConnectionType { get; set; }
        public string MailConnectionString { get; set; }

        public int BackendBasePort { get; set; } = 5000;

    }

    public enum ConnectionType
    {
        FileSystem,
        Oracle
    }

    public class OracleSessionParameters
    {
        public string ClientId { get; set; }
        public string ClientInfo { get; set; }
        public string ModuleName { get; set; }
        public string DateLanguage { get; set; }
    }
}
