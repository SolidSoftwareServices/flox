using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.Serialization;
using Ei.Rp.DomainModels.Billing;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.DomainServices.IntegrationTests.Infrastructure;
using EI.RP.DomainServices.Queries.Billing.Activity;
using EI.RP.DomainServices.Queries.Contracts.Accounts;
using NUnit.Framework;

namespace EI.RP.DomainServices.IntegrationTests.Queries.Billing
{
	[Explicit("TODO")]
	public class AccountBillingActivityQueryTests : DomainTests
	{
		public static IEnumerable CanGetAccountBillsCases()
		{
			yield return new TestCaseData("RodV@VanceTest.com", "Munna@123");
			yield return new TestCaseData("hop.mark@test.com", "Test1234");
			yield return new TestCaseData("DFEqual902033859@esb.ie", "Test3333");

			yield return new TestCaseData("ebtwoproducts@pp.ie", "Test3333");
			yield return new TestCaseData("ebcancellation@pp.ie", "Test3333");
			yield return new TestCaseData("eoinsmithh@pp.ie", "Test1111");
			yield return new TestCaseData("multipleelec@esb.ie", "Test3333");
		}
		
		[Test,TestCaseSource(nameof(CanGetAccountBillsCases))]
		public async Task CanGetAccountBills(string userName, string password)
		{
			await LoginUser(userName, password);
			var userAccounts =await DomainQueryProvider
				.FetchAsync<AccountInfoQuery, AccountInfo>(
					new AccountInfoQuery());
			foreach (var account in userAccounts)
			{
				var actual = (await DomainQueryProvider.FetchAsync<AccountBillingActivityQuery, AccountBillingActivity>(
					new AccountBillingActivityQuery
						{AccountNumber = account.AccountNumber,Source = AccountBillingActivity.ActivitySource.Invoice}))
					.OrderBy(x => x.OriginalDate).ToArray();

				Assert.IsNotNull(actual);
				Assert.IsTrue(actual.All(x=>x.Source==AccountBillingActivity.ActivitySource.Invoice));
				Console.WriteLine($"Retrieved {actual.Count()} models:{actual.ToJson()}");
			}
		}


		public static IEnumerable CanGetAccountActivityCases()
		{
			yield return new TestCaseData("multipleelec@esb.ie", "Test3333", "903946230");
		}
		
		[Test, TestCaseSource(nameof(CanGetAccountActivityCases))]
		public async Task CanGetAccountBillingActivity(string userName, string password, string accountNumber)
		{
			await LoginUser(userName, password);
			var actual = (await DomainQueryProvider.FetchAsync<AccountBillingActivityQuery, AccountBillingActivity>(
					new AccountBillingActivityQuery
						{AccountNumber = accountNumber})).OrderByDescending(x => x.OriginalDate)
				.Select(x => new
				{
					x.OriginalDate,x.Description,DueAmount = x.CurrentBalanceAmount,x.ReceivedAmount,x.InvoiceFileAvailable
				}).ToArray();

			Console.WriteLine($"Retrieved {actual.Count()} models:{actual.ToJson()}");

		}


		public static IEnumerable OverdueInvoicesCases()
		{
			yield return new TestCaseData("Overdue1bill@gmail.ie", "Test3333", "901819501");
		}

		[Test, TestCaseSource(nameof(OverdueInvoicesCases))]
		public async Task OverdueInvoices(string userName, string password, string accountNumber)
		{
			await LoginUser(userName, password);
			var actual = (await DomainQueryProvider.FetchAsync<AccountBillingActivityQuery, AccountBillingActivity>(
					new AccountBillingActivityQuery
						{ AccountNumber = accountNumber,Source=AccountBillingActivity.ActivitySource.Invoice }))
				.OrderByDescending(x => x.OriginalDate)
				.Select(x => new
				{
					x.OriginalDate,
					x.Description,
					x.CurrentBalanceAmount,
					x.Amount,
					x.ReceivedAmount,
					x.InvoiceFileAvailable,
					x.InvoiceStatus
				}).ToArray();

			Console.WriteLine($"Retrieved {actual.Count()} models:{actual.ToJson()}");

		}
	}
}
