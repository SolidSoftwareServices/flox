using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using Moq;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.A_MovingHouse.Step3
{
	[TestFixture]
	class WhenInMovingHomeStep3Page_MoveGasAddElectricity_Test : MyAccountCommonTests<Step3InputMoveInPropertyDetailsPage>
	{

		private Step2InputPrnsPage _step2InputPrns;
		protected override async Task TestScenarioArrangement()
		{
			UserConfig = App.ConfigureUser("a@A.com", "test");
			
			UserConfig
				.AddGasAccount(
					configureDefaultDevice:false,
					newPrnAddressExists: true,
					canAddNewAccount:true
				)
				.WithGasDevice()
				
				;

			UserConfig.Execute();

            await ((ResidentialPortalApp) await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
			var movingHomeLandingPage = (await App.CurrentPageAs<MyAccountElectricityAndGasPage>().ToMovingHome())
				.CurrentPageAs<Step0LandingPage>();

			var step1Page =
				(await movingHomeLandingPage.ClickOnElement(movingHomeLandingPage.PopupButton2))
				.CurrentPageAs<Step1InputMoveOutPage>()
				.InputFormValues(UserConfig);

			_step2InputPrns = (await step1Page.ClickOnElement(step1Page.GetNextPRNButton()))
				.CurrentPageAs<Step2InputPrnsPage>();

			_step2InputPrns.InputFormValues(UserConfig);

			var step2Page2 = (await _step2InputPrns.ClickOnElement(_step2InputPrns.SubmitPRNS)).CurrentPageAs<Step2ConfirmAddressPage>();
			Sut = (await step2Page2.ClickOnElement(step2Page2.ButtonContinue))
				.CurrentPageAs<Step3InputMoveInPropertyDetailsPage>();
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

		[Test]
		public async Task ContractSaleValidatorExecuted()
		{
			App.DomainFacade.QueryResolver.Current.Setup(x =>
				x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(
					It.Is<MovingHouseValidationQuery>(_ => _.ValidateContractSaleChecks || _.NewMPRN == _step2InputPrns.MPRNInput.Value), It.IsAny<bool>()))
				.ReturnsAsync(new MovingHouseRulesValidationResult
				{
					Output = OutputType.Failed,
					MovingHouseValidationType = MovingHouseValidationType.IsContractSaleChecksOk
				}.ToOneItemArray());

			var step3Page = Sut.InputFormValues(UserConfig);
			var page = await step3Page.ClickOnElement(step3Page.NextPaymentOptions);
			page.CurrentPageAs<ShowMovingHouseValidationErrorPage>();
		}
	}
}