using Elfo.Wardein.Core.Abstractions;
using Elfo.Wardein.Core.Helpers;
using Elfo.Wardein.Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Core.ConfigurationReader
{
    public class MailConfigurationReader : IAmMailConfigurationReader
    {
        private readonly string filePath;

        public MailConfigurationReader(string filePath)
        {
            this.filePath = filePath;
        }

        public MailConfiguration GetMailConfiguration() =>
            JsonConvert.DeserializeObject<MailConfiguration>(new IOHelper(filePath).GetFileContentFromPath());
    }
}
