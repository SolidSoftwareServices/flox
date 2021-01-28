using System.Linq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step3
{
	[TestFixture]
    class WhenInMovingHomeStep3Page_OnlyGas_Test : MyAccountCommonTests<Step3InputMoveInPropertyDetailsPage>
    {
        protected override async Task TestScenarioArrangement()
        {

            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddGasAccount();
            UserConfig.Execute();

            var withValidSessionFor = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            var movingHomeLandingPage = (await withValidSessionFor.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome()).CurrentPageAs<Step0LandingPage>();

            var step1Page = (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton1))
                .CurrentPageAs<Step1InputMoveOutPage>();

            await step1Page.InputFormValues(UserConfig).ClickOnElement(step1Page.GetNextPRNButton());
            var step2InputPrnsPage = App.CurrentPageAs<Step2InputPrnsPage>();
            step2InputPrnsPage.GPRNInput.Value = UserConfig.ElectricityAndGasAccountConfigurators
                .FirstOrDefault(x => x.Model.ClientAccountType == ClientAccountType.Gas)
                ?.NewPremise.GasPrn.ToString();
            await step2InputPrnsPage.ClickOnElement(step2InputPrnsPage.SubmitPRNS);
            var step2ConfirmAddressPage = App.CurrentPageAs<Step2ConfirmAddressPage>();
            await step2ConfirmAddressPage.ClickOnElement(step2ConfirmAddressPage.ButtonContinue);
            Sut = App.CurrentPageAs<Step3InputMoveInPropertyDetailsPage>();
        }

        [Test]
        public async Task CanSeeComponents()
        {
			Sut.AssertInitialViewComponents(UserConfig);
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
            Assert.AreEqual("Gas", Sut.GasMeterReadingsHeader.TextContent);

            var gprn = UserConfig.ElectricityAndGasAccountConfigurators
	            .FirstOrDefault(x => x.Model.ClientAccountType == ClientAccountType.Gas)
	            ?.NewPremise.GasPrn.ToString();

            Assert.IsTrue(Sut.GprnHeader.TextContent.Contains(gprn));

            (await Sut.ClickOnElement(Sut.NextPaymentOptions)).CurrentPageAs<Step3InputMoveInPropertyDetailsPage>();

            Assert.IsTrue(Sut.GprnHeader.TextContent.Contains(gprn));
            Assert.IsTrue(Sut.GasMeterReadingDescription.TextContent.Contains("We will need the meter reading at your new home from the day you move in to make sure you are billed correctly from the start."));
		}

        [Test]
        public async Task TermsAndConditionsCheckBoxIsStillVisibleOnValidationFailure()
        {
	        Sut.AssertHasCorrectReadGeneralTermsLink();
	        await Sut.ClickOnElement(Sut.NextPaymentOptions);
	        App.CurrentPageAs<Step3InputMoveInPropertyDetailsPage>().AssertHasCorrectReadGeneralTermsLink();
        }

		
	}
}