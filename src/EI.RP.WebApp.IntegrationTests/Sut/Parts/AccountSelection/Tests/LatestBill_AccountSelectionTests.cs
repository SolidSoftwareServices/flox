using System.Linq;
using System.Threading.Tasks;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.EstimateYourCost;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MakeAPayment;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Tests
{
	class LatestBill_AccountSelectionTests : WebAppPageTests<AccountSelectionPage>
	{
		private AppUserConfigurator _userConfig;
		protected override async Task TestScenarioArrangement()
		{
			_userConfig = App.ConfigureUser("a@A.com", "test");
			_userConfig.AddElectricityAccount(canBeRefunded: true);
			_userConfig.AddElectricityAccount();
			_userConfig.AddElectricityAccount(opened: false);

			_userConfig.AddElectricityAccount(canEstimateLatestBill: true);
			_userConfig.Execute();
			Sut = ((ResidentialPortalApp)await App.WithValidSessionFor(_userConfig.UserName, _userConfig.Role)).CurrentPageAs<AccountSelectionPage>();
		}

		[TestCase(0)]
		[TestCase(1)]
		public async Task CanClickTheMakeAPaymentBtn_FromLatestBill(int index)
		{
            (await Sut.ClickMakeAPayment(index)).CurrentPageAs<MakeAPaymentPage>();
        }

        [Test]
		public async Task CanClickOnEstimateLink_FromLatestBill_ThenClickOnBillsAndPayments()
		{
            var actual = (await Sut.ClickEstimateButton()).CurrentPageAs<EstimateYourCostPage>();

            //ThenClickOnBillsAndPayments: bug:23679
			(await actual.ToBillingAndPayments()).CurrentPageAs<ShowPaymentsHistoryPage>();
		}

	
	}
}