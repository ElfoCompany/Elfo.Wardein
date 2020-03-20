using Elfo.Firmenich.Wardein.Abstractions.Watchers;
using Elfo.Firmenich.Wardein.Abstractions.WebWatcher;
using Elfo.Firmenich.Wardein.Core.ServiceManager;
using Elfo.Wardein.Core.Helpers;
using Elfo.Wardein.Integrations.Oracle.Integration;
using Elfo.Wardein.Watchers.WebWatcher;
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

            connectionString = configuration["ConnectionStrings:Db"];

            OracleConnectionConfiguration = new OracleConnectionConfiguration.Builder(connectionString)
                .WithClientId(configuration["Oracle:ClientId"])
                .WithClientInfo(configuration["Oracle:ClientInfo"])
                .WithModuleName(configuration["Oracle:ModuleName"])
                .WithDateLanguage(configuration["Oracle:DateLanguage"])
                .Build();

            oracleIntegration = new OracleIntegration(OracleConnectionConfiguration);
            watcherPersistenceService = ServicesContainer.WatcherPersistenceService(connectionString);
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

