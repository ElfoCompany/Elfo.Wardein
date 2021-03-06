﻿using Elfo.Wardein.Core.Abstractions;
using Elfo.Wardein.Core.Helpers;
using Elfo.Wardein.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Core.ConfigurationReader
{
    public class MailConfigurationManagerFromJSON : IAmMailConfigurationManager
    {
        private readonly string filePath;
        private MailConfiguration cachedMailConfiguration;

        public MailConfigurationManagerFromJSON(string filePath)
        {
            this.filePath = filePath;
            this.cachedMailConfiguration = null;
        }

        public MailConfiguration GetConfiguration()
        {
            if(cachedMailConfiguration == null)
                cachedMailConfiguration = JsonConvert.DeserializeObject<MailConfiguration>(new IOHelper(filePath).GetFileContent());

            return cachedMailConfiguration;
        }

        public void InvalidateCache()
        {
            this.cachedMailConfiguration = null;
        }
    }
}
