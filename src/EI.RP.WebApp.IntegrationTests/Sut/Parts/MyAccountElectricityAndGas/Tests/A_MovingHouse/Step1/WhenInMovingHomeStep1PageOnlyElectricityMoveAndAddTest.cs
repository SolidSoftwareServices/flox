using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step1
{
    [TestFixture]
    class WhenInMovingHomeStep1PageOnlyElectricityMoveAndAddTest : MyAccountCommonTests<Step1InputMoveOutPage>
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount().WithElectricity24HrsDevices();
            UserConfig.Execute();
            var withValidSessionFor = await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            var movingHomeLandingPage = (await withValidSessionFor.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome()).CurrentPageAs<Step0LandingPage>();

            Sut = (await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton2)).CurrentPageAs<Step1InputMoveOutPage>();
        }

        [Test]
        public async Task TermsAndConditionsCheckBoxIsStillVisibleOnValidationFailure()
        {
	        AssertHasCorrectReadGeneralTermsLink(Sut);
			await Sut.ClickOnElement(Sut.GetNextPRNButton());
	        AssertHasCorrectReadGeneralTermsLink(App.CurrentPageAs<Step1InputMoveOutPage>());
        }

        private void AssertHasCorrectReadGeneralTermsLink(Step1InputMoveOutPage page)
        {
	        Assert.IsNotNull(page.CheckBoxTerms);
	        var electricityLink = page.GeneralTermsAndConditionsLinks.Single();
	        Assert.IsTrue(electricityLink.Attributes["href"].Value.ToLowerInvariant() == "https://electricireland.ie/residential/helpful-links/terms-conditions/residential-electricity", "expected to see link to terms and conditions for electricity");
        }
    }
}