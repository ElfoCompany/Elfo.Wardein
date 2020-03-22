using Elfo.Wardein.Core.ConfigurationManagers;
using Elfo.Wardein.Abstractions.Configuration.Models;
using Elfo.Wardein.Core.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elfo.Wardein.Core.Tests
{
    [TestClass]
    public class OracleWardeinConfigurationManagerTest
    {

        [TestMethod]
        public void CanReadConfigurations()
        {
            var oracleHelper = Substitute.For<IOracleHelper>();
            oracleHelper.Query<WardeinConfigurationModel>(Arg.Any<string>(), Arg.Any<IDictionary<string, object>>()).Returns(new List<WardeinConfigurationModel>()
            {
                new WardeinConfigurationModel() { WatcherConfigurationId = 1, ApplicationHostname = "SRVWEB07", ApplicationId = 1, WardeinConfig = "{\"timeSpanFromSeconds\":60}", WatcherTypeJsonConfig = "{}", WatcherJsonConfig = "{\"services\":[{\"serviceName\":\"MSMQ\",\"maxRetryCount\":2,\"serviceManagerType\":0}]}" },
                new WardeinConfigurationModel() { WatcherConfigurationId = 2, ApplicationHostname = "SRVWEB07", ApplicationId = 1, WardeinConfig = "{\"timeSpanFromSeconds\":60}", WatcherTypeJsonConfig = "{}", WatcherJsonConfig = "{\"iisPools\":[{\"serviceName\":\"Elfo.Wardein\",\"maxRetryCount\":2,\"serviceManagerType\":1}]}" }
            });
            var manager = new OracleWardeinConfigurationManager(oracleHelper, "SRVWEB07");
            var configs = manager.GetConfiguration();
            Assert.AreEqual(60, configs.TimeSpanFromSeconds);
            Assert.AreEqual(1, configs.IISPools.Count());
            Assert.AreEqual(1, configs.Services.Count());
        }
    }
}
