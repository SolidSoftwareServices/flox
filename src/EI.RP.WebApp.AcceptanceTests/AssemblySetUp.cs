using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.Drivers;
using EI.RP.WebApp.AcceptanceTests.Infrastructure.NUnitExtensions;
using NUnit.Framework;

namespace EI.RP.WebApp.AcceptanceTests
{
	[SetUpFixture]
	public class AssemblySetUp
	{


		[OneTimeSetUp]
		public async Task SetUp()
		{
			await DeleteLogs();
		}


		[OneTimeTearDown]
		public async Task TearDown()
		{
			await DeleteLogs();

			DriversPool.Default.Value.Dispose();
		}

		private async Task DeleteLogs()
		{
			var a = DeleteLog($"{TestSettings.Default.PublicTargetUrl}/LogView/Delete");
			var b = DeleteLog($"{TestSettings.Default.InternalTargetUrl}/LogView/Delete");
			await Task.WhenAll(a, b);
		}

		public async Task DeleteLog(string urlDelete)
		{
			var handler = new HttpClientHandler
			{
				ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true,
				DefaultProxyCredentials = CredentialCache.DefaultCredentials,
			};
			using (var client = new HttpClient(handler))
			{
				client.DefaultRequestHeaders.Accept.Clear();
				var result = await client.GetAsync(urlDelete);
				result.EnsureSuccessStatusCode();
				string value = await result.Content.ReadAsStringAsync();
				TestContext.Progress.WriteOutputLine("Log Deleted");
			}
		}
	}
}