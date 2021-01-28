using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using EI.RP.DataServices;
using Ei.Rp.DomainModels.Billing;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using EI.RP.DomainServices.Queries.Billing.InvoiceFiles;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Queries.Billing
{
	[Explicit("TODO")]
	public class InvoiceFileQueryTests : DomainTests
	{


		public static IEnumerable QueryReturnsFileCorrectlyCases()
		{
			yield return new TestCaseData("multipleelec@esb.ie", "Test3333", "903946230", "1406876320");

		}
		[Test, TestCaseSource(nameof(QueryReturnsFileCorrectlyCases))]
		public async Task QueryReturnsFileCorrectly(string userName, string password,string accountNumber,string referenceDocNumber)
		{
			await LoginUser(userName, password);
			var result = (await DomainQueryProvider
				.FetchAsync<InvoiceFileQuery, InvoiceFile>(new InvoiceFileQuery
				{
					ReferenceDocNumber = referenceDocNumber,AccountNumber = accountNumber
				})).SingleOrDefault();


			Assert.IsNotNull(result);
			Assert.IsTrue(result.FileData.Length>1024*2);

#if DEBUG
			File.WriteAllBytes(result.GetFileName(),result.FileData);


#endif

		}
		[Test, TestCaseSource(nameof(QueryReturnsFileCorrectlyCases))]
		public  Task QueryPublishesLogEventsCorrectly(string userName, string password, string invoiceId)
		{
			throw new NotImplementedException();

		}

		public static IEnumerable RepositoryReturnsFileCorrectlyCases()
		{
			yield return new TestCaseData("6387831");

		}
		[Test, TestCaseSource(nameof(RepositoryReturnsFileCorrectlyCases))]
		public async Task RepositoryReturnsFileCorrectly(string documentNumber)
		{
			var repository=AssemblySetUp.Container.Value.Resolve<IStreamServeRepository>();
			using (var src=await (await repository.GetInvoiceFileStream(documentNumber)).ReadAsStreamAsync())
			{
				Assert.IsTrue(src.Length>1024*2,"Doc must be at least 10KB");
				
			}

			
		}
	}
}