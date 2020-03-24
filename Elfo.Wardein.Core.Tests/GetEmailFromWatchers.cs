using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Abstractions.Services;
using Elfo.Wardein.Abstractions.Watchers;
using Elfo.Wardein.Core.Helpers;
using Elfo.Wardein.Integrations.Oracle.Integration;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Elfo.Wardein.Core.Tests
{
    [TestClass]
    public class GetEmailFromWatchers
    {
        private string connectionString = string.Empty;
        OracleConnectionConfiguration OracleConnectionConfiguration;
        private OracleIntegration oracleIntegration;
        private  IAmWatcherPersistenceService watcherPersistenceService;
        private IAmNotificationService notificationService;

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
            notificationService = ServicesContainer.NotificationService(Abstractions.NotificationType.Mail);
        }

        [TestMethod]
        [TestCategory("ManualTest")]
        public async Task IsEmailWasSent()
        {
            Task result = null;
            var svc = await watcherPersistenceService.UpsertCurrentStatus(1, 2, "SRVWEB07", false);
            if (svc.PreviousStatus == false)
            {
               result = notificationService.SendNotificationAsync("yaroslav.husynin@elfo.net;", "Restore was Successful test Message",                           
                   $"Attention");
            }
      
            Assert.IsNotNull(result);
        }
     }
}
