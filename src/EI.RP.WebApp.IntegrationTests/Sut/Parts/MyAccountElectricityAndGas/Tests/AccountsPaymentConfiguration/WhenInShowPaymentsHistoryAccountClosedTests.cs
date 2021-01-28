using System.Threading.Tasks;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AccountsPaymentConfiguration
{
	internal class WhenInShowPaymentsHistoryAccountClosedTests : WhenInAccountPaymentsConfigurationTests
	{
		protected override async Task TestScenarioArrangement()
		{
			SetIsAccountOpen(false);
			SetHasInvoices(false);
			await base.TestScenarioArrangement();
		}

		[Test]
		public async Task ShowUiForClosedAccount()
		{
			Assert.IsNull(Sut.Overview.HowToReadLink);
			Assert.AreEqual("No payments have been applied to this account yet.", Sut.Overview.NoPaymentAppliedMessage?.TextContent);
			Assert.IsNull(Sut.Overview.PaymentMessage);
		}
	}
}