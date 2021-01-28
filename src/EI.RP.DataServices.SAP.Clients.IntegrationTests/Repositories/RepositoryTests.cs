using System;
using System.IO;
using System.Threading.Tasks;
using EI.RP.DataServices.SAP.Clients.IntegrationTests.Infrastructure;
using EI.RP.DataServices.SAP.Clients.Repositories;
using NUnit.Framework;

namespace EI.RP.DataServices.SAP.Clients.IntegrationTests.Repositories
{
	internal abstract class RepositoryTests : SapTest
	{
		protected async Task DoLogin()
		{
			await LoginUser("elecdd33@esb.ie", "Test9999");
		}

		protected abstract SapRepository GetRepository();

		
		[Test]
		public async Task CanGetMetadataAsEdmx()
		{
			await DoLogin();
			var sapRepository = GetRepository();
			var actual = await sapRepository.ResolveMetadata();
			Assert.IsFalse(string.IsNullOrWhiteSpace(actual.RawEdm));
			Assert.IsNotNull(actual.EdmModel);
			Console.WriteLine(actual);

		}

	}
}
