using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Elfo.Wardein.Abstractions
{
    public static class HostHelper
    {
        public static string GetName()
        {
#if DEBUG
            return "SRVWEB06";
#else
            return Dns.GetHostName()?.ToUpperInvariant();
#endif
        }
    }
}
