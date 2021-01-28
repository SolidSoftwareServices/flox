using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step3
{
	[TestFixture]
	class WhenInMovingHomeStep3PageMoveElectricityAndAddGasTest : MyAccountCommonTests<Step3InputMoveInPropertyDetailsPage>
	{
		protected override async Task TestScenarioArrangement()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");
			UserConfig
				.AddElectricityAccount(configureDefaultDevice: false,canAddNewAccount:true,newPrnAddressExists:true)
				.WithElectricity24HrsDevices()
				;

			UserConfig.Execute();

			var withValidSessionFor = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
			var movingHomeLandingPage = (await withValidSessionFor.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome()).CurrentPageAs<Step0LandingPage>();
			 
			var step1Page = (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton2)).CurrentPageAs<Step1InputMoveOutPage>();
			step1Page.InputFormValues(UserConfig);
			var step2Page = (await step1Page.ClickOnElement(step1Page.GetNextPRNButton())).CurrentPageAs<Step2InputPrnsPage>();
			step2Page.InputFormValues(UserConfig);
			var step2ConfirmAddressPagePage = (await step2Page.ClickOnElement(step2Page.SubmitPRNS)).CurrentPageAs<Step2ConfirmAddressPage>();
			await step2ConfirmAddressPagePage.ClickOnElement(step2ConfirmAddressPagePage.ButtonContinue);
			Sut = App.CurrentPageAs<Step3InputMoveInPropertyDetailsPage>();
		}

		[Test]
		public async Task TermsAndConditionsCheckBoxIsStillVisibleOnValidationFailure()
		{
			Sut.AssertHasCorrectReadGeneralTermsLink();
			await Sut.ClickOnElement(Sut.NextPaymentOptions);
			App.CurrentPageAs<Step3InputMoveInPropertyDetailsPage>().AssertHasCorrectReadGeneralTermsLink();
		}
		[Test]
		public async Task CanSeeComponents()
		{
			Sut.AssertInitialViewComponents(UserConfig);
		}
	}
}