using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AccountsPaymentConfiguration
{
	[TestFixture]
	internal class WhenInShowPaymentsHistoryEqualizerPaymentTests : WhenInAccountPaymentsConfigurationTests
	{
		protected override PaymentMethodType PaymentMethodType => PaymentMethodType.Equalizer;

		protected override async Task TestScenarioArrangement()
		{
			SetHasInvoices(true);
			await base.TestScenarioArrangement();
		}

		[Test]
		public async Task EqualizerDoesNotShowOverduePayment()
		{
			Assert.IsNull(Sut.PaymentOverDue);
		}
	}
}