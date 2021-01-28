using System.Linq;
using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step3
{
	[TestFixture]
	class WhenInMovingHomeStep3PageDuelFuelFromElectricityMoveAndCloseTest : MyAccountCommonTests<Step3InputMoveInPropertyDetailsPage>
	{
        protected override async Task TestScenarioArrangement()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");
			UserConfig
				.AddElectricityAccount(configureDefaultDevice: false).WithElectricity24HrsDevices()
				;

			UserConfig.AddGasAccount(duelFuelSisterAccount: UserConfig.ElectricityAndGasAccountConfigurators.Single())
				;

			UserConfig.Execute();

			await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role);
			await App.CurrentPageAs<AccountSelectionPage>().SelectAccount(UserConfig.Accounts.Last().AccountNumber);
			await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome();
			await App.CurrentPageAs<Step0LandingPage>().ClickOnElement(App.CurrentPageAs<Step0LandingPage>().PopupButton2);

			App.CurrentPageAs<Step1InputMoveOutPage>().InputFormValues(UserConfig);

			await App.CurrentPageAs<Step1InputMoveOutPage>().ClickOnElement(App.CurrentPageAs<Step1InputMoveOutPage>().GetNextPRNButton());
			App.CurrentPageAs<Step2InputPrnsPage>().InputFormValues(UserConfig);
			await App.CurrentPageAs<Step2InputPrnsPage>().ClickOnElement(App.CurrentPageAs<Step2InputPrnsPage>().SubmitPRNS);
			await App.CurrentPageAs<Step2ConfirmAddressPage>().ClickOnElement(App.CurrentPageAs<Step2ConfirmAddressPage>().ButtonContinue);
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
			Sut.AssertInitialViewComponents(UserConfig,true);
		}
	}
}