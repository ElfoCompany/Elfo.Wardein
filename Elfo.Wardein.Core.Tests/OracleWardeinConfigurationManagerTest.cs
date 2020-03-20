using Elfo.Firmenich.Wardein.Core.ConfigurationManagers;
using Elfo.Wardein.Core.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elfo.Wardein.Core.Tests
{
    [TestClass]
    public class OracleWardeinConfigurationManagerTest
    {
        private string connectionString = string.Empty;
        private IConfiguration configuration;
        private OracleConnectionConfiguration config;

        [TestInitialize]
        public void Initialize()
        {
            this.configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

            connectionString = configuration["ConnectionStrings:Db"];

            this.config = new OracleConnectionConfiguration.Builder(this.connectionString)
                .WithClientId(configuration["Oracle:ClientId"])
                .WithClientInfo(configuration["Oracle:ClientInfo"])
                .WithModuleName(configuration["Oracle:ModuleName"])
                .WithDateLanguage(configuration["Oracle:DateLanguage"])
                .Build();
        }

        [TestMethod]
        [TestCategory("ManualTest")]
        public void CanReadConfigurations()
        {
            var manager = new OracleWardeinConfigurationManager(config, "SRVWEB07");
            var configs = manager.GetConfiguration();
            Assert.IsNotNull(configs);
        }
    }
}
