using System.Threading.Tasks;
using Elfo.Wardein.Integrations.Oracle.Integration;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elfo.Wardein.Integrations.Tests
{
    [TestClass]
    public class GetDataFromSourceTest     
    {
		private  string connectionString = null;
		[TestInitialize]
		public void Initialize()
		{
			var configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.Build();
			connectionString = configuration["ConnectionString:Db"];
		}


		[TestMethod]
		[TestCategory("ManualTest")]
		public async Task ConnectionAreOk()
		{
			OracleIntegrationConfiguration config = new OracleIntegrationConfiguration(connectionString);

			OracleIntegration connection = new OracleIntegration(config);

			var result = await connection.QueryAsync<object>("Select * from DUMMY_WRD");

			Assert.IsNotNull(result);
		}

		[TestMethod]
		[TestCategory("ManualTest")]
		public async Task InsertIsOk()
		{
			OracleIntegrationConfiguration config = new OracleIntegrationConfiguration(connectionString);

			OracleIntegration connection = new OracleIntegration(config);

			var result = await connection.ExecuteAsync("INSERT INTO DUMMY_WRD VALUES (33, 'Some text')");

			Assert.IsNotNull(result);
		}

		[TestMethod]
		[TestCategory("ManualTest")]
		public async Task UpdateIsOk()
		{
			OracleIntegrationConfiguration config = new OracleIntegrationConfiguration(connectionString);

			OracleIntegration connection = new OracleIntegration(config);

			var result = await connection.ExecuteAsync("UPDATE DUMMY_WRD SET  Id = 20, Text = 'Some text' WHERE Id = 1");

			Assert.IsNotNull(result);
		}
	}
}
