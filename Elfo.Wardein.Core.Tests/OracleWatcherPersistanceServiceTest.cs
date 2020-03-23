using Elfo.Wardein.Abstractions.Watchers;
using Elfo.Wardein.Core.Helpers;
using Elfo.Wardein.Integrations.Oracle.Integration;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Elfo.Wardein.Core.Tests
{
    [TestClass]
    public class OracleWatcherPersistanceServiceTest
    {

        private string connectionString = string.Empty;
        OracleConnectionConfiguration OracleConnectionConfiguration;
        private OracleIntegration oracleIntegration;
        private IAmWatcherPersistenceService watcherPersistenceService;

        [TestInitialize]
        public void Initialize()
        {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

            connectionString = configuration["StorageConnectionString"];

            WardeinBaseConfiguration wbc = new WardeinBaseConfiguration();
            configuration.Bind(wbc);
            ServicesContainer.Initialize(wbc);

            OracleConnectionConfiguration = new OracleConnectionConfiguration.Builder(connectionString)
                .WithClientId(configuration["OracleAdditionalParams:ClientId"])
                .WithClientInfo(configuration["OracleAdditionalParams:ClientInfo"])
                .WithModuleName(configuration["OracleAdditionalParams:ModuleName"])
                .WithDateLanguage(configuration["OracleAdditionalParams:DateLanguage"])
                .Build();
            oracleIntegration = new OracleIntegration(OracleConnectionConfiguration);
            watcherPersistenceService = ServicesContainer.WatcherPersistenceService();
        }

        [TestMethod]
        [TestCategory("ManualTest")]
        public async Task IsErrorCountRiseWithFalse()
        {
            var svc = await watcherPersistenceService.UpsertCurrentStatus(1, 2, "SRVWEB07", false);

            Assert.IsTrue(svc.FailureCount > 0);
        }

        [TestMethod]
        [TestCategory("ManualTest")]
        public async Task IsErrorCountEqualToZeroWithTrue()
        {
            var svc = await watcherPersistenceService.UpsertCurrentStatus(1, 2, "SRVWEB07", true);

            Assert.IsTrue(svc.FailureCount == 0);
        }
    }
}

