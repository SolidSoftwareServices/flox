using System;
using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step3
{

    [TestFixture]
    class WhenInMovingHomeStep3PageDuelFuelFromElectricityTest : MyAccountCommonTests<Step3InputMoveInPropertyDetailsPage>
    {
	    protected override async Task TestScenarioArrangement()
	    {

		    UserConfig = App.ConfigureUser("a@A.com", "test");
		    UserConfig.AddElectricityAccount(configureDefaultDevice: false).WithElectricity24HrsDevices();
			UserConfig.AddGasAccount(duelFuelSisterAccount: UserConfig.ElectricityAndGasAccountConfigurators.Single());
		    UserConfig.Execute();

		    await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role);
		    await App.CurrentPageAs<AccountSelectionPage>()
			    .SelectAccount(UserConfig.Accounts.Last().AccountNumber);
		    await App
			    .CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome();
		    var movingHomeLandingPage = App.CurrentPageAs<Step0LandingPage>();

		    var step1Page = (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton1))
			    .CurrentPageAs<Step1InputMoveOutPage>();
            step1Page.InputFormValues(UserConfig);
		    var step2Page = (await step1Page.ClickOnElement(step1Page.GetNextPRNButton())).CurrentPageAs<Step2InputPrnsPage>();
            step2Page.InputFormValues(UserConfig);
            var step2ConfirmAddressPagePage = (await step2Page.ClickOnElement(step2Page.SubmitPRNS)).CurrentPageAs<Step2ConfirmAddressPage>();
            await step2ConfirmAddressPagePage.ClickOnElement(step2ConfirmAddressPagePage.ButtonContinue);
            Sut = App.CurrentPageAs<Step3InputMoveInPropertyDetailsPage>();
        }

        [Test]
        public async Task CanSeeComponents()
        {
           
            Sut.AssertInitialViewComponents(UserConfig);
            
        }

      
        [Test]
        public async Task TermsAndConditionsCheckBoxIsStillVisibleOnValidationFailure()
        {
	        Sut.AssertHasCorrectReadGeneralTermsLink();
	        await Sut.ClickOnElement(Sut.NextPaymentOptions);
	        App.CurrentPageAs<Step3InputMoveInPropertyDetailsPage>().AssertHasCorrectReadGeneralTermsLink();
        }

		[Test]
        public async Task WhenClickOnCancel()
        {
            Assert.IsNotNull(Sut.CancelButton);
            Assert.IsTrue(Sut.CancelButton.TextContent.Contains("Cancel"));

            var sutLandingPage = (await Sut.ClickOnElement(Sut.CancelMovePage))
                .CurrentPageAs<Step0LandingPage>();
        }

        [Test]
        public async Task ThrowsValidations_Fields_Empty()
        {
            Assert.AreEqual("Electricity", Sut.ElectricityMeterReadingsHeader.TextContent);
            Assert.AreEqual("Gas", Sut.GasMeterReadingsHeader.TextContent);
            Assert.IsTrue(Sut.MprnHeader.TextContent.Contains(UserConfig.ElectricityAndGasAccountConfigurators.FirstOrDefault(x => x.Model.ClientAccountType == ClientAccountType.Electricity)?.NewPremise.ElectricityPrn.ToString()));

            var gprn = UserConfig.GasAccount().NewPremise.GasPrn.ToString();
            Assert.IsTrue(Sut.GprnHeader.TextContent.Contains(gprn));

			(await Sut.ClickOnElement(Sut.NextPaymentOptions)).CurrentPageAs<Step3InputMoveInPropertyDetailsPage>();

			var mprn = UserConfig.ElectricityAccount().NewPremise.ElectricityPrn.ToString();
            Assert.IsTrue(Sut.MprnHeader.TextContent.Contains(mprn));

            Assert.IsTrue(Sut.GprnHeader.TextContent.Contains(gprn));
            Assert.IsTrue(Sut.GasMeterReadingDescription.TextContent.Contains("We will need the meter reading at your new home from the day you move in to make sure you are billed correctly from the start."));
            Assert.IsTrue(Sut.ElectricityMeterReadingDescription.TextContent.Contains("We will need the meter reading at your new home from the day you move in to make sure you are billed correctly from the start."));
		}
    }
}