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
				["JSON_CNFG"] = new OracleParameter("JSON_CNFG", OracleDbType.Clob).Value = "{}",
				["WTCHR_CNFG_ID"] = new OracleParameter("WTCHR_CNFG_ID", OracleDbType.Int32).Value = 1
			};

			string query = "UPDATE WRD_WTCHR_CNFG SET JSON_CNFG = :JSON_CNFG WHERE WTCHR_CNFG_ID = :WTCHR_CNFG_ID";

			var result  = await oracleIntegration.ExecuteAsync(query, updateDateParameter);
			Assert.IsNotNull(result);
		}
	}
}
