using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step1
{
	[TestFixture]
	class WhenInMovingHomeStep1Page_DuelFuelFromElectricityMoveElectricityAndCloseGasTest : MyAccountCommonTests<Step1InputMoveOutPage>
	{
		protected override async Task TestScenarioArrangement()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");
			UserConfig.AddElectricityAccount().WithElectricity24HrsDevices();
			UserConfig.AddGasAccount(duelFuelSisterAccount: UserConfig.ElectricityAndGasAccountConfigurators.Single());
			UserConfig.Execute();

			await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role);
			await App.CurrentPageAs<AccountSelectionPage>().SelectAccount(UserConfig.Accounts.Last().AccountNumber);
			await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome();
			await App.CurrentPageAs<Step0LandingPage>().ClickOnElement(App.CurrentPageAs<Step0LandingPage>().PopupButton2);
			Sut = App.CurrentPageAs<Step1InputMoveOutPage>();
		}

		[Test]
		public async Task CanSeeComponents()
		{
			AssertHasCorrectReadGeneralTermsLink(Sut);
        }

		private void AssertHasCorrectReadGeneralTermsLink(Step1InputMoveOutPage page)
		{
			Assert.IsNotNull(page.CheckBoxTerms);
			var links = page.GeneralTermsAndConditionsLinks.ToArray();
			var electricityLink = links[0];
			var gasLink = links[1];
			Assert.IsTrue(electricityLink.Attributes["href"].Value.ToLowerInvariant() == "https://electricireland.ie/residential/helpful-links/terms-conditions/residential-electricity", "expected to see link to terms and conditions for electricity");
			Assert.IsTrue(gasLink.Attributes["href"].Value.ToLowerInvariant() == "https://electricireland.ie/residential/helpful-links/terms-conditions/residential-gas", "expected to see link to terms and conditions for gas");
		}
	}
}