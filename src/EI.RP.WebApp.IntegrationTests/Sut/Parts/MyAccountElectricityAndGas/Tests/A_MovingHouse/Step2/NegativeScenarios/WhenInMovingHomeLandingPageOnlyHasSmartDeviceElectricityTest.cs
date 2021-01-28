using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using Moq;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step2.NegativeScenarios
{
	[TestFixture]
	class WhenInMovingHomeLandingPageOnlyHasSmartDeviceElectricityTest : WhenInMovingHomeNegativeScenarioPageTest<ShowMovingHouseValidationErrorPage>
	{
		protected override async Task TestScenarioArrangement()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");
			UserConfig.AddElectricityAccount()
				.WithAddNewAccountAvailable(true, false);
			UserConfig.Execute();

			var movingHouseValidationQueryResult = new MovingHouseRulesValidationResult
			{
				Output = OutputType.Failed,
				MovingHouseValidationType = MovingHouseValidationType.IsNonSmartMoveInDeviceSet,
			};

			await ((ResidentialPortalApp)await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
			var movingHomeLandingPage = (await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome())
				.CurrentPageAs<Step0LandingPage>();

			var step1Page =
				(await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton1))
				.CurrentPageAs<Step1InputMoveOutPage>()
				.InputFormValues(UserConfig);

			var step2Page1 =
				(await step1Page.ClickOnElement(step1Page.GetNextPRNButton()))
				.CurrentPageAs<Step2InputPrnsPage>();

			step2Page1.InputFormValues(UserConfig);

			App.DomainFacade.QueryResolver.Current.Setup(
					x => x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
				.Returns(Task.FromResult(movingHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));

			Sut =
				(await step2Page1.ClickOnElement(step2Page1.SubmitPRNS)).CurrentPageAs<ShowMovingHouseValidationErrorPage>();
		}

		[Test]
		public override async Task CanSeeComponents()
		{
			Assert.IsTrue(Sut.ErrorMessageBody.TextContent.Contains("To move your account to an address with a smart meter, please call customer service on 1850 372 372 from 8am - 8pm, Monday - Saturday."));
			Assert.IsTrue(Sut.ErrorMessageTitle.TextContent.Contains("We cannot process your move request at the moment."));
		}
	}
}