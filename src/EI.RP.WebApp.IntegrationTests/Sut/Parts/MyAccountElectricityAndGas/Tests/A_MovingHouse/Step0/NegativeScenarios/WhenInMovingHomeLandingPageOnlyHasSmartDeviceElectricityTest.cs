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

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step0.NegativeScenarios
{
	[TestFixture]
	class WhenInMovingHomeLandingPageOnlyHasSmartDeviceElectricityTest : WhenInMovingHomeNegativeScenarioPageTest<ShowMovingHouseValidationErrorPage>
	{
		protected override async Task TestScenarioArrangement()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");
			UserConfig.AddElectricityAccount();
			UserConfig.Execute();

			var movingHouseValidationQueryResult = new MovingHouseRulesValidationResult
			{
				Output = OutputType.Failed,
				MovingHouseValidationType = MovingHouseValidationType.IsNonSmartMoveOutDeviceSet
			};

			UserConfig.DomainFacade.QueryResolver.Current.Setup(
					x => x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
				.Returns(Task.FromResult(movingHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));

			var withValidSessionFor = await ((ResidentialPortalApp)await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
			var a = await withValidSessionFor.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome();
			Sut = a.CurrentPageAs<ShowMovingHouseValidationErrorPage>();
		}

		[Test]
		public override async Task CanSeeComponents()
		{
			Assert.IsTrue(Sut.ErrorMessageBody.TextContent.Contains("To move your account to a different address, please call customer service on 1850 372 372 from 8am - 8pm, Monday - Saturday."));
			Assert.IsTrue(Sut.ErrorMessageTitle.TextContent.Contains("We cannot process your move request at the moment."));
		}
	}
}