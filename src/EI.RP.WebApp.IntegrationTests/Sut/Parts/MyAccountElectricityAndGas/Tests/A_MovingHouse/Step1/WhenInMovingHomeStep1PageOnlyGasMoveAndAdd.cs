using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.ErrorHandling;
using Ei.Rp.DomainErrors;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using Moq;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step1
{
	[TestFixture]
	class WhenInMovingHomeStep1PageOnlyGasMoveAndAdd : MyAccountCommonTests<Step1InputMoveOutPage>
	{
		protected override async Task TestScenarioArrangement()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");
			UserConfig.AddGasAccount();
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
			var gasLink = page.GeneralTermsAndConditionsLinks.Single();
			Assert.IsTrue(gasLink.Attributes["href"].Value.ToLowerInvariant() == "https://electricireland.ie/residential/helpful-links/terms-conditions/residential-gas", "expected to see link to terms and conditions for gas");
		}

        [Test]
        public async Task ShowErrorPage_Pod_Locked()
        {
            var step1Page = App.CurrentPageAs<Step1InputMoveOutPage>();
            step1Page = step1Page.InputFormValues(UserConfig);

            UserConfig.DomainFacade.QueryResolver.Current.Setup(_ =>
                    _.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
                .Throws(new DomainException(ResidentialDomainError.PoDLocked));

            var page = await step1Page.ClickOnElement(step1Page.GetNextPRNButton());
            var sutErrorPage = page.CurrentPageAs<MovingHouseUnhandledErrorPage>();

            Assert.IsNotNull(sutErrorPage.Title);
            Assert.IsNotNull(sutErrorPage.Subtitle);

            Assert.IsNotNull(sutErrorPage.ContactNumber);
            Assert.IsTrue(sutErrorPage.ContactNumber.TextContent.Contains("Please call 1850 372 372 to complete your move"));

            Assert.IsNotNull(sutErrorPage.BackToAccounts);
            Assert.IsTrue(sutErrorPage.BackToAccounts.TextContent.Contains("Back to My Accounts"));
            (await sutErrorPage.ClickOnElement(sutErrorPage.BackToAccounts))
                .CurrentPageAs<AccountSelectionPage>();
        }
	}
}