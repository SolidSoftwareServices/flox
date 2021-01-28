using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.System;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.UI.TestServices.Flows.FlowScreenUnitTest;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps;
using EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Infrastructure.StepsDataBuilder;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Steps
{
	[TestFixture]
	internal class Step2ConfirmAddressTests
    {
		private FlowScreenTestConfigurator<Step2ConfirmAddress, ResidentialPortalFlowType> NewScreenTestConfigurator(DomainFacade domainFacade = null)
		{
			if (domainFacade == null) domainFacade = new DomainFacade();

			return new FlowScreenTestConfigurator<Step2ConfirmAddress, ResidentialPortalFlowType>(domainFacade.ModelsBuilder)
				//Assigns the domain stubs to the configurator to be injected in the step instances(see other methods)
				.WithStub(domainFacade.SessionProvider)
				.WithStub(domainFacade.QueryResolver)
				.WithStub(domainFacade.CommandDispatcher);
		}

		private static IEnumerable<TestCaseData> MovingHouseTestCases(string caseName)
		{
			var movingHouseTypes = MovingHouseType.AllValues.Where(x =>
					!x.IsOneOf(MovingHouseType.CloseElectricityAndGas, MovingHouseType.CloseElectricity,
						MovingHouseType.CloseGas))
				.Cast<MovingHouseType>();

			foreach (var movingType in movingHouseTypes)
			{
				//compose the test case
				yield return new TestCaseData(new AccountsForMovingTypeBuilder(movingType))
					.SetName($"{caseName}_{movingType}");
			}
		}

		[TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(ItHandles_ErrorEvent) })]
		public async Task ItHandles_ErrorEvent(AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
		{
			var domainConfiguration = accountsForMovingTypeBuilder.Create();
			var rootDataBuilder = new RootDataBuilder(domainConfiguration, accountsForMovingTypeBuilder.MovingType);

			var step0OperationSelectionDataBuilder = new Step0OperationSelectionDataBuilder(domainConfiguration, rootDataBuilder);
			var step1DataBuilder = new Step1InputMoveOutPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder, step0OperationSelectionDataBuilder);
			var step2DataBuilder = new Step2InputPrnsDataBuilder(domainConfiguration, rootDataBuilder, step0OperationSelectionDataBuilder, step1DataBuilder);
			var step2ConfirmDataBuilder = new Step2ConfirmAddressDataBuilder(domainConfiguration, rootDataBuilder);

			var rootStepData = rootDataBuilder.Create();
			var step0Data = step0OperationSelectionDataBuilder.Create();
			var step1Data = step1DataBuilder.Create();
			var step2Data = step2DataBuilder.Create();
			var step2ConfirmData = step2ConfirmDataBuilder.Create();

			NewScreenTestConfigurator(domainConfiguration.DomainFacade)
				//get a runner to test events
				.NewEventTestRunner(step2ConfirmData)
				//arrange the scenario by setting the user selections in the previous steps
				.WithExistingStepData(ScreenName.PreStart, rootStepData)
				.WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)
				.WithExistingStepData(MovingHouseStep.Step1InputMoveOutPropertyDetails, step1Data)
				.WithExistingStepData(MovingHouseStep.Step2InputPrns, step2Data)
				.WhenEvent(ScreenEvent.ErrorOccurred)
				//assert UI
				.ThenTheResultStepIs(MovingHouseStep.ShowMovingHouseUnhandledError);
		}

		[TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(ItHandlesEvent_To_ToStep3) })]
		public void ItHandlesEvent_To_ToStep3(AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
		{
            var domainConfiguration = accountsForMovingTypeBuilder.Create();

            //create builders that help creating reusable scenario parts
            var rootDataBuilder = new RootDataBuilder(domainConfiguration, accountsForMovingTypeBuilder.MovingType);

            var step0OperationSelectionDataBuilder =
                new Step0OperationSelectionDataBuilder(domainConfiguration, rootDataBuilder);
            var step1DataBuilder =
                new Step1InputMoveOutPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder,
                    step0OperationSelectionDataBuilder);
            var step2DataBuilder =
                new Step2InputPrnsDataBuilder(domainConfiguration, rootDataBuilder,
                    step0OperationSelectionDataBuilder, step1DataBuilder);

            //build the state datas
            var rootStepData = rootDataBuilder.Create();
            var step0Data = step0OperationSelectionDataBuilder.Create();
            var step1Data = step1DataBuilder.Create();
            var step2Data = step2DataBuilder.Create();

            var movinHouseValidationQueryResult = new MovingHouseRulesValidationResult
            {
                Output = OutputType.Passed,
                MovingHouseValidationType = MovingHouseValidationType.PodNotRegisteredEarlierToday
			};

            //domainConfigurator.DomainFacade.QueryResolver.ExpectQuery. TODO ND: does not seem to be triggering IEquatable logic in query
            domainConfiguration.DomainFacade.QueryResolver.Current.Setup(
                x => x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(movinHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));

            //build the unit test configurator
            NewScreenTestConfigurator(domainConfiguration.DomainFacade)
                //get a runner to test events
                .NewEventTestRunner(step2Data)
                //arrange the scenario by setting the user selections in the previous steps
                .WithExistingStepData(ScreenName.PreStart, rootStepData)
                .WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)
                .WithExistingStepData(MovingHouseStep.Step1InputMoveOutPropertyDetails, step1Data)

            //act
            .WhenEvent(Step2ConfirmAddress.StepEvent.ToStep3)

            //assert UI
            .ThenTheResultStepIs(MovingHouseStep.Step3InputMoveInPropertyDetails)
            .ThenTheValidationPassed();
        }

        [TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(ItHandlesEvent_To_ValidateNewPremisePodNotRegisteredTodayError) })]
        public void ItHandlesEvent_To_ValidateNewPremisePodNotRegisteredTodayError(AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
        {
            var domainConfiguration = accountsForMovingTypeBuilder.Create();

            //create builders that help creating reusable scenario parts
            var rootDataBuilder = new RootDataBuilder(domainConfiguration, accountsForMovingTypeBuilder.MovingType);

            var step0OperationSelectionDataBuilder =
                new Step0OperationSelectionDataBuilder(domainConfiguration, rootDataBuilder);
            var step1DataBuilder =
                new Step1InputMoveOutPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder,
                    step0OperationSelectionDataBuilder);
            var step2DataBuilder =
                new Step2InputPrnsDataBuilder(domainConfiguration, rootDataBuilder,
                    step0OperationSelectionDataBuilder, step1DataBuilder);

            //build the state datas
            var rootStepData = rootDataBuilder.Create();
            var step0Data = step0OperationSelectionDataBuilder.Create();
            var step1Data = step1DataBuilder.Create();
            var step2Data = step2DataBuilder.Create();

            var movinHouseValidationQueryResult = new MovingHouseRulesValidationResult
            {
                Output = OutputType.Failed,
                MovingHouseValidationType = MovingHouseValidationType.PodNotRegisteredEarlierToday
			};

            //domainConfigurator.DomainFacade.QueryResolver.ExpectQuery. TODO ND: does not seem to be triggering IEquatable logic in query
            domainConfiguration.DomainFacade.QueryResolver.Current.Setup(
                x => x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(movinHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));

            //build the unit test configurator
            NewScreenTestConfigurator(domainConfiguration.DomainFacade)
                //get a runner to test events
                .NewEventTestRunner(step2Data)
                //arrange the scenario by setting the user selections in the previous steps
                .WithExistingStepData(ScreenName.PreStart, rootStepData)
                .WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)
                .WithExistingStepData(MovingHouseStep.Step1InputMoveOutPropertyDetails, step1Data)

            //act
            .WhenEvent(Step2ConfirmAddress.StepEvent.ToStep3)

            //assert UI
            .ThenTheResultStepIs(MovingHouseStep.ShowMovingHouseValidationError)
            .ThenTheValidationPassed();
        }

		[Test]
        public void FlowIsCorrect()
        {
            Assert.AreEqual(ResidentialPortalFlowType.MovingHouse, NewScreenTestConfigurator().Adapter.GetFlowType());
        }

        [Test]
        public void ScreenStepIsCorrect()
        {
            Assert.AreEqual(MovingHouseStep.Step2ConfirmAddress,
                NewScreenTestConfigurator().Adapter.GetStep());
        }

        [Test]
        public void ViewPathIsCorrect()
        {
            Assert.AreEqual("Step2ConfirmAddress", NewScreenTestConfigurator().Adapter.GetViewPath());
        }
    }
}