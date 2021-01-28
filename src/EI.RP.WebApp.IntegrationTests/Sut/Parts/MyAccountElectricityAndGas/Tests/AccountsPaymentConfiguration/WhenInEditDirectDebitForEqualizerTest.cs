using System.Linq;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration.DirectDebit;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Plan;
using NUnit.Framework;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainServices.Commands.Banking.DirectDebit.SetUpDirectDebit;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.AccountsPaymentConfiguration
{
	[TestFixture]
	internal class WhenInEditDirectDebitForEqualizerTest : WhenInAccountPaymentsConfigurationTests
	{
		private PlanPage _sut;
		protected override async Task TestScenarioArrangement()
		{
			SetPaymentMethodType(PaymentMethodType.Equalizer);
			await base.TestScenarioArrangement();
			_sut = (await Sut.ClickOnElement(Sut.Overview.BillAndPaymentOptionsLink)).CurrentPageAs<PlanPage>();
		}


		[Test]
		public async Task WhenElectricIrelandIbanUpdated()
		{
			var iban = "IE62AIBK93104777372010";
			var bankName = "AIB";
			var pageEdit = (await _sut.ClickOnElement(_sut.EditDirectDebitLink)).CurrentPageAs<InputDirectDebitDetailsPage>();
			pageEdit.Iban.Value = iban;
			pageEdit.CustomerName.Value = bankName;
			pageEdit.ConfirmTerms.IsChecked = true;
			(await Sut.ClickOnElement(pageEdit.UpdateDetailsButton)).CurrentPageAs<DirectDebitPageConfirmation>();
		}
	}
}
