using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Elfo.Wardein.Core.Helpers
{
    public static class Const
    {
        public static readonly string BASE_PATH = AppDomain.CurrentDomain.BaseDirectory;

        public static readonly string SERVER_NAME = System.Environment.MachineName;

        public static readonly string DB_PATH = $@"{Const.BASE_PATH}Assets\WardeinDB.json";

        public static readonly string LOG_PATH = Path.Combine(BASE_PATH, "nlog.config");
    }
}
