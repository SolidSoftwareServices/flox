using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AccountsPaymentConfiguration
{
	[TestFixture]
	internal class WhenInShowPaymentsHistoryDirectDebitPaymentTests : WhenInAccountPaymentsConfigurationTests
	{
		protected override PaymentMethodType PaymentMethodType => PaymentMethodType.DirectDebit;
		protected override async Task TestScenarioArrangement()
		{
			SetHasInvoices(true);
			SetHasOverDueInvoice();
			await base.TestScenarioArrangement();
		}

		[Test]
		public async Task DoesShowOverDuePayment()
		{
			Assert.IsNotNull(Sut.PaymentOverDue);
		}
	}
}