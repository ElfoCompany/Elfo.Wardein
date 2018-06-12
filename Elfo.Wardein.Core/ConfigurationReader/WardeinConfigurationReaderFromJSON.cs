using Elfo.Wardein.Core.Abstractions;
using Elfo.Wardein.Core.Helpers;
using Elfo.Wardein.Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Core.ConfigurationReader
{
    public class WardeinConfigurationReaderFromJSON : IAmWardeinConfigurationReaderService
    {
        private readonly string wardeinConfigurationPath;
        private WardeinConfig cachedWardeinConfig;

        public WardeinConfigurationReaderFromJSON(string wardeinConfigurationPath = "../Assets/WardeinConfig.json")
        {
            this.wardeinConfigurationPath = wardeinConfigurationPath;
        }

        public WardeinConfig GetConfiguration()
        {
            if (this.cachedWardeinConfig == null)
                this.cachedWardeinConfig = JsonConvert.DeserializeObject<WardeinConfig>(new IOHelper(this.wardeinConfigurationPath).GetFileContent());

            return cachedWardeinConfig;
        }

        public void InvalidateCache() => this.cachedWardeinConfig = null;
    }
}
