using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.StepCloseConfirmation
{
    [TestFixture]
    class WhenInMovingHomeStepCloseConfirmationPage_DuelFuelFromElectricityTest : MyAccountCommonTests<StepCloseAccountPageConfirmationPage>
    {
        protected override async Task TestScenarioArrangement()
        {

            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount(paymentType: PaymentMethodType.DirectDebit,configureDefaultDevice:false).WithElectricity24HrsDevices();
            UserConfig.AddGasAccount(paymentType: PaymentMethodType.DirectDebit, duelFuelSisterAccount: UserConfig.ElectricityAndGasAccountConfigurators.Single());
            UserConfig.Execute();
          
            await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role);
            await App.CurrentPageAs<AccountSelectionPage>()
	            .SelectAccount(UserConfig.Accounts.Last().AccountNumber);
             await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome();
            var movingHomeLandingPage = App.CurrentPageAs<Step0LandingPage>();

            var stepClosePage = (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton3))
                .CurrentPageAs<StepCloseAccountPage>();
            stepClosePage = FillRequiredFields(stepClosePage);
            Sut = (await stepClosePage.ClickOnElement(stepClosePage.GetCloseButton()))
				.CurrentPageAs<StepCloseAccountPageConfirmationPage>();
		}
        [Test]
        public async Task CanSeeComponents()
        {
	        var accountConfigurators = UserConfig.ElectricityAndGasAccountConfigurators.ToArray();
	        Assert.IsTrue(Sut.GetElectricityAccountInfo().Text().Contains("Electricity"));
	        Assert.IsTrue(Sut.GetElectricityAccountInfo().Text().Contains(accountConfigurators[0].Model.AccountNumber));
	        Assert.IsTrue(Sut.GetGasAccountInfo().Text().Contains("Gas"));
	        Assert.IsTrue(Sut.GetGasAccountInfo().Text().Contains(accountConfigurators[1].Model.AccountNumber));
	        Assert.AreEqual($"Your Electricity bill will be paid direct debit.", Sut.GetElectricityPaymentInfo().TextContent);
	        Assert.AreEqual($"Your Gas bill will be paid direct debit.", Sut.GetGasPaymentInfo().TextContent);

		}

        private StepCloseAccountPage FillRequiredFields(StepCloseAccountPage stepClosePage)
        {
	        var accountConfigurators = UserConfig.ElectricityAndGasAccountConfigurators.ToArray();
	        var elecDevicesMeterReading = accountConfigurators[0].Premise.ElectricityDevice().Registers.Single();
	        var gasDevicesMeterReading = accountConfigurators[1].Premise.GasDevice().Registers.Single();

			stepClosePage.IsROIFieldRequired.Value = "true";
			stepClosePage.GetRoiHouseNumberInputElement().Value = "HouseNumber";
			stepClosePage.GetRoiStreetInputElement().Value = "Street";
			stepClosePage.GetRoiTownInputElement().Value = "Town";
			stepClosePage.GetRoiRoiCountyDropDownElementOption().OuterHtml = "<option selected=\"selected\" value=\"CK\">Cork</option>";

			stepClosePage.GetElectricityReadingInput(elecDevicesMeterReading.MeterNumber).Value = "123";
            stepClosePage.GetGasReadingInput(gasDevicesMeterReading.MeterNumber).Value = "123";
            stepClosePage.CheckBoxDetails.IsChecked = true;
            stepClosePage.CheckBoxTerms.IsChecked = true;

			return stepClosePage;
        }

		[Test]
		public async Task CanGoToAccountsPageAfterClickBackToAccounts()
		{
			var backToMyAccountsLink = Sut.BackToMyAccountsLink;
			Assert.NotNull(backToMyAccountsLink);

			var accountsPage = (await Sut.ClickOnElement(backToMyAccountsLink))
							.CurrentPageAs<AccountSelectionPage>();

			Assert.NotNull(accountsPage);
		}

	}
}
