using Elfo.Wardein.Integrations.Oracle.Integration;
using Elfo.Wardein.Oracle;
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
		[TestInitialize]
		public void Initialize()
		{
			var configuration = new ConfigurationBuilder()
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			.Build();

			connectionString = configuration["ConnectionStrings:Db"];
		}

		[TestMethod]
		[TestCategory("ManualTest")]
		public async Task ConnectionAreOk()
		{
			OracleConnectionConfiguration config = new OracleConnectionConfiguration(connectionString);

			OracleIntegration connection = OracleIntegration.Create(config);

			var result = await connection.QueryAsync<object>("Select * from DUMMY_WRD");

			Assert.IsNotNull(result);
		}

		[TestMethod]
		[TestCategory("ManualTest")]
		public async Task InsertIsOk()
		{
			OracleConnectionConfiguration config = new OracleConnectionConfiguration(connectionString);

			OracleIntegration connection = OracleIntegration.Create(config);

			var result = await connection.ExecuteAsync("INSERT INTO DUMMY_WRD VALUES (55, 'Some text22')");

			Assert.IsNotNull(result);
		}

		[TestMethod]
		[TestCategory("ManualTest")]
		public async Task SelectAreOk()
		{
			OracleConnectionConfiguration config = new OracleConnectionConfiguration(connectionString);

			OracleIntegration connection = OracleIntegration.Create(config);

			var result = await connection.ExecuteAsync("SELECT * FROM DUMMY_WRD");

			Assert.IsNotNull(result);
		}

		[TestMethod]
		[TestCategory("ManualTest")]
		public async Task UpdateIsOk()
		{
			OracleConnectionConfiguration config = new OracleConnectionConfiguration(connectionString);

			OracleIntegration connection = OracleIntegration.Create(config);

			var updateDateParameter = new Dictionary<string, object>
			{
				["DT_LAST_HB"] = new OracleParameter("DT_LAST_HB", OracleDbType.Date).Value = DateTime.UtcNow,
				["APPL_HOSTNAME"] = new OracleParameter("APPL_HOSTNAME", OracleDbType.Varchar2).Value = "SRVWEB07"
			};

			string query = "UPDATE WRD_CNFG SET DT_LAST_HB = :DT_LAST_HB WHERE APPL_HOSTNAME = :APPL_HOSTNAME";

			var result  = await connection.ExecuteAsync(query, updateDateParameter);
			Assert.IsNotNull(result);
		}
	}
}
