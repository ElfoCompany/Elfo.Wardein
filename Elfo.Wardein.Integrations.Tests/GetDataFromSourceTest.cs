using Elfo.Wardein.Core.Helpers;
using Elfo.Wardein.Integrations.Oracle.Integration;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elfo.Wardein.Integrations.Tests
{
    [TestClass]
    public class GetDataFromSourceTest     
    {
		private string connectionString = string.Empty;
		OracleConnectionConfiguration OracleConnectionConfiguration;
		OracleIntegration oracleIntegration;
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
		}

		[TestMethod]
		[TestCategory("ManualTest")]
		public async Task ConnectionAreOk()
		{
			

			var result = await oracleIntegration.QueryAsync<object>("Select * from DUMMY_WRD");

			Assert.IsNotNull(result);
		}

		[TestMethod]
		[TestCategory("ManualTest")]
		public async Task InsertIsOk()
		{
			var result = await oracleIntegration.ExecuteAsync("INSERT INTO DUMMY_WRD VALUES (55, 'Some text22')");

			Assert.IsNotNull(result);
		}

		[TestMethod]
		[TestCategory("ManualTest")]
		public async Task SelectAreOk()
		{
			var result = await oracleIntegration.ExecuteAsync("SELECT * FROM DUMMY_WRD");

			Assert.IsNotNull(result);
		}

		[TestMethod]
		[TestCategory("ManualTest")]
		public async Task UpdateIsOk()
		{
			var updateDateParameter = new Dictionary<string, object>
			{
				["DT_LAST_HB"] = new OracleParameter("DT_LAST_HB", OracleDbType.Date).Value = DateTime.UtcNow,
				["APPL_HOSTNAME"] = new OracleParameter("APPL_HOSTNAME", OracleDbType.Varchar2).Value = "SRVWEB07"
			};

			string query = "UPDATE WRD_CNFG SET DT_LAST_HB = :DT_LAST_HB WHERE APPL_HOSTNAME = :APPL_HOSTNAME";

			var result  = await oracleIntegration.ExecuteAsync(query, updateDateParameter);
			Assert.IsNotNull(result);
		}
	}
}
