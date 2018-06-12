using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Core.Helpers
{
    public static class Const
    {
        public static readonly string BASE_PATH = AppDomain.CurrentDomain.BaseDirectory;

        public static readonly string SERVER_NAME = System.Environment.MachineName;

        public static readonly string WARDEIN_CONFIG_PATH = $@"{Const.BASE_PATH}Assets\WardeinConfig.json";

        public static readonly string DB_PATH = $@"{Const.BASE_PATH}Assets\WardeinDB.json";
    }
}
