using Elfo.Wardein.Core.ConfigurationManagers;
using Elfo.Wardein.Oracle;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elfo.Wardein.Core.Test
{
    [TestClass]
    public class WardeinConfigurationManagerFromOracleTest
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
            var manager = new WardeinConfigurationManagerFromOracle(config);
            var configs = manager.GetConfiguration();
            Assert.IsNotNull(configs);
        }
    }
}
