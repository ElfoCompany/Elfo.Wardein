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
            return Dns.GetHostName()?.ToUpperInvariant();
        }
    }
}
