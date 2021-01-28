using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.Commands.LongRunningFlows.MovingHouse;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.UI.TestServices.Flows.FlowScreenUnitTest;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps;
using EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Infrastructure.StepsDataBuilder;
using Moq;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Steps
{
	[TestFixture]
	internal class Step1InputMoveOutPropertyDetailsTests
	{
		private FlowScreenTestConfigurator<Step1InputMoveOutPropertyDetails, ResidentialPortalFlowType>
			NewScreenTestConfigurator(DomainFacade domainFacade = null)
		{
			//creates the domain layer facade
			if (domainFacade == null) domainFacade = new DomainFacade();
			//creates a flo test configurator
			return new FlowScreenTestConfigurator<Step1InputMoveOutPropertyDetails, ResidentialPortalFlowType>(domainFacade.ModelsBuilder)
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
			var domainConfigurator = accountsForMovingTypeBuilder.Create();
	        var rootDataBuilder = new RootDataBuilder(domainConfigurator, accountsForMovingTypeBuilder.MovingType);

	        var step0OperationSelectionDataBuilder = new Step0OperationSelectionDataBuilder(domainConfigurator, rootDataBuilder);
	        var step1InputMoveOutPropertyDetailsDataBuilder = new Step1InputMoveOutPropertyDetailsDataBuilder(domainConfigurator, rootDataBuilder, step0OperationSelectionDataBuilder);

			var rootStepData = rootDataBuilder.Create();
			var step1Data = step1InputMoveOutPropertyDetailsDataBuilder.Create();

			NewScreenTestConfigurator(domainConfigurator.DomainFacade)
		        //get a runner to test events
		        .NewEventTestRunner(step1Data)
		        .WithExistingStepData(ScreenName.PreStart, rootStepData)
		        .WhenEvent(ScreenEvent.ErrorOccurred)

		        //assert UI
		        .ThenTheResultStepIs(MovingHouseStep.ShowMovingHouseUnhandledError);
        }

        [TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(CreateStepData_SetsAcknowledgementType) })]
        public void CreateStepData_SetsAcknowledgementType(
			AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
		{
			var domainConfigurator = accountsForMovingTypeBuilder.Create();
			//create scenario state
			var rootDataBuilder = new RootDataBuilder(domainConfigurator, accountsForMovingTypeBuilder.MovingType);
			var rootStepData = rootDataBuilder.Create();
			var step0OperationSelectionData =
				new Step0OperationSelectionDataBuilder(domainConfigurator, rootDataBuilder).Create();

			//create the test configurator
			NewScreenTestConfigurator(domainConfigurator.DomainFacade)
				//get a runner to test the step creation
				.NewTestCreateStepDataRunner()
				//arrange the scenario preconditions
				.WithExistingStepData(ScreenName.PreStart, rootStepData)
				.WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0OperationSelectionData)
				//act, it executes the creation of the step
				.WhenCreated()
				//assert 
				.ThenTheStepDataIs<Step1InputMoveOutPropertyDetails.ScreenModel>(actual =>
				{
					var expected = ShowTermsAcknowledgmentType.None;
					if (step0OperationSelectionData.MovingHouseType.IsOneOf(MovingHouseType.MoveElectricityAndCloseGas))
						expected = ShowTermsAcknowledgmentType.ShowBoth;

					if (step0OperationSelectionData.MovingHouseType.IsOneOf(MovingHouseType.MoveGasAndAddElectricity))
						expected = ShowTermsAcknowledgmentType.ShowGas;

					if (step0OperationSelectionData.MovingHouseType.IsOneOf(MovingHouseType.MoveElectricityAndAddGas))
						expected = ShowTermsAcknowledgmentType.ShowElectricity;

					Assert.AreEqual(expected,
						actual.ShowTermsAcknowledgmentType);
				});
		}

        [TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(ItHandlesEvent_To_MovingHouseValidationOnPhoneNumberIsValidFailed) })]
        public void ItHandlesEvent_To_MovingHouseValidationOnPhoneNumberIsValidFailed(AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
		{
			var domainConfiguration = accountsForMovingTypeBuilder.Create();
			
			//create builders that help creating reusable scenario parts
			var rootDataBuilder = new RootDataBuilder(domainConfiguration, accountsForMovingTypeBuilder.MovingType);
			var step0OperationSelectionDataBuilder =
				new Step0OperationSelectionDataBuilder(domainConfiguration, rootDataBuilder);
			var step1DataBuilder =
				new Step1InputMoveOutPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder,
					step0OperationSelectionDataBuilder);

			//build the state datas
			var rootStepData = rootDataBuilder.Create();
			var step0Data = step0OperationSelectionDataBuilder.Create();
			var step1Data = step1DataBuilder.Create();

            var movinHouseValidationQueryResult = new MovingHouseRulesValidationResult
            {
                Output = OutputType.Failed,
                MovingHouseValidationType = MovingHouseValidationType.PhoneNumberIsValid
            };

            domainConfiguration.DomainFacade.QueryResolver.Current.Setup(
                    x => x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(movinHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));

            //build the unit test configurator
            NewScreenTestConfigurator(domainConfiguration.DomainFacade)
				//get a runner to test events
				.NewEventTestRunner(step1Data)
				//arrange the scenario by setting the user selections in the previous steps
				.WithExistingStepData(ScreenName.PreStart, rootStepData)
				.WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)

				//act
				.WhenEvent(Step1InputMoveOutPropertyDetails.StepEvent.ToStep2)

				//assert UI
				.ThenTheResultStepIs(MovingHouseStep.ShowMovingHouseValidationError)
				.ThenTheValidationPassed();

			//other domain assertions, like the domain as told to record the progress
			domainConfiguration.DomainFacade.CommandDispatcher.AssertCommandWasExecuted<RecordMovingOutProgress>();

		}

        [TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(ItHandlesEvent_To_ToStep2OnPhoneNumberIsValidPassed) })]
        public void ItHandlesEvent_To_ToStep2OnPhoneNumberIsValidPassed(AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
		{
			var domainConfiguration = accountsForMovingTypeBuilder.Create();
			
			//create builders that help creating reusable scenario parts
			var rootDataBuilder = new RootDataBuilder(domainConfiguration, accountsForMovingTypeBuilder.MovingType);
			var step0OperationSelectionDataBuilder =
				new Step0OperationSelectionDataBuilder(domainConfiguration, rootDataBuilder);
			var step1DataBuilder =
				new Step1InputMoveOutPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder,
					step0OperationSelectionDataBuilder);

			//build the state datas
			var rootStepData = rootDataBuilder.Create();
			var step0Data = step0OperationSelectionDataBuilder.Create();
			var step1Data = step1DataBuilder.Create();

            var movinHouseValidationQueryResult = new MovingHouseRulesValidationResult
            {
                Output = OutputType.Passed,
                MovingHouseValidationType = MovingHouseValidationType.PhoneNumberIsValid
            };

            domainConfiguration.DomainFacade.QueryResolver.Current.Setup(
                    x => x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(movinHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));

            //build the unit test configurator
            NewScreenTestConfigurator(domainConfiguration.DomainFacade)
				//get a runner to test events
				.NewEventTestRunner(step1Data)
				//arrange the scenario by setting the user selections in the previous steps
				.WithExistingStepData(ScreenName.PreStart, rootStepData)
				.WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)

				//act
				.WhenEvent(Step1InputMoveOutPropertyDetails.StepEvent.ToStep2)

				//assert UI
				.ThenTheResultStepIs(MovingHouseStep.Step2InputPrns)
				.ThenTheValidationPassed();

			//other domain assertions, like the domain as told to record the progress
			domainConfiguration.DomainFacade.CommandDispatcher.AssertCommandWasExecuted<RecordMovingOutProgress>();

		}

        [TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(ItHandlesEvent_To_ToStep2OnHasAccountDevicesPassed) })]
        public void ItHandlesEvent_To_ToStep2OnHasAccountDevicesPassed(AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
        {
            var domainConfiguration = accountsForMovingTypeBuilder.Create();

            //create builders that help creating reusable scenario parts
            var rootDataBuilder = new RootDataBuilder(domainConfiguration, accountsForMovingTypeBuilder.MovingType);

            var step0DataBuilder =
                new Step0OperationSelectionDataBuilder(domainConfiguration, rootDataBuilder);
            var step1DataBuilder =
                new Step1InputMoveOutPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder,
                    step0DataBuilder);

            //build the state datas
            var rootStepData = rootDataBuilder.Create();
            var step0Data = step0DataBuilder.Create();
            var step1Data = step1DataBuilder.Create();

            var movinHouseValidationQueryResult = new MovingHouseRulesValidationResult
            {
                Output = OutputType.Passed,
                MovingHouseValidationType = MovingHouseValidationType.HasAccountDevices
            };

            //domainConfigurator.DomainFacade.QueryResolver.ExpectQuery. TODO : does not seem to be triggering IEquatable logic in query
            domainConfiguration.DomainFacade.QueryResolver.Current.Setup(
                x => x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(movinHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));

            //build the unit test configurator
            NewScreenTestConfigurator(domainConfiguration.DomainFacade)
                //get a runner to test events
                .NewEventTestRunner(step1Data)
                //arrange the scenario by setting the user selections in the previous steps
                .WithExistingStepData(ScreenName.PreStart, rootStepData)
                .WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)

                //act
                .WhenEvent(Step1InputMoveOutPropertyDetails.StepEvent.ToStep2)

                //assert UI
                .ThenTheResultStepIs(MovingHouseStep.Step2InputPrns)
                .ThenTheValidationPassed();

            //other domain assertions, like the domain as told to record the progress
            domainConfiguration.DomainFacade.CommandDispatcher.AssertCommandWasExecuted<RecordMovingOutProgress>();
        }

        [TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(ItHandlesEvent_To_MovingHouseValidationOnHasAccountDevicesFailed) })]
        public void ItHandlesEvent_To_MovingHouseValidationOnHasAccountDevicesFailed(AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
        {
            var domainConfiguration = accountsForMovingTypeBuilder.Create();

            //create builders that help creating reusable scenario parts
            var rootDataBuilder = new RootDataBuilder(domainConfiguration, accountsForMovingTypeBuilder.MovingType);

            var step0DataBuilder =
                new Step0OperationSelectionDataBuilder(domainConfiguration, rootDataBuilder);
            var step1DataBuilder =
                new Step1InputMoveOutPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder,
                    step0DataBuilder);

            //build the state datas
            var rootStepData = rootDataBuilder.Create();
            var step0Data = step0DataBuilder.Create();
            var step1Data = step1DataBuilder.Create();

            var movinHouseValidationQueryResult = new MovingHouseRulesValidationResult
            {
                Output = OutputType.Failed,
                MovingHouseValidationType = MovingHouseValidationType.HasAccountDevices
            };

            //domainConfigurator.DomainFacade.QueryResolver.ExpectQuery. TODO : does not seem to be triggering IEquatable logic in query
            domainConfiguration.DomainFacade.QueryResolver.Current.Setup(
                x => x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(movinHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));

            //build the unit test configurator
            NewScreenTestConfigurator(domainConfiguration.DomainFacade)
                //get a runner to test events
                .NewEventTestRunner(step1Data)
                //arrange the scenario by setting the user selections in the previous steps
                .WithExistingStepData(ScreenName.PreStart, rootStepData)
                .WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)

                //act
                .WhenEvent(Step1InputMoveOutPropertyDetails.StepEvent.ToStep2)

                //assert UI
                .ThenTheResultStepIs(MovingHouseStep.ShowMovingHouseValidationError)
                .ThenTheValidationPassed();

            //other domain assertions, like the domain as told to record the progress
            domainConfiguration.DomainFacade.CommandDispatcher.AssertCommandWasExecuted<RecordMovingOutProgress>();
        }

        [TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(ItHandlesEvent_To_ToStep2OnCanCloseAccountsPassed) })]
        public void ItHandlesEvent_To_ToStep2OnCanCloseAccountsPassed(AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
        {
            var domainConfiguration = accountsForMovingTypeBuilder.Create();

            //create builders that help creating reusable scenario parts
            var rootDataBuilder = new RootDataBuilder(domainConfiguration, accountsForMovingTypeBuilder.MovingType);

            var step0DataBuilder =
                new Step0OperationSelectionDataBuilder(domainConfiguration, rootDataBuilder);
            var step1DataBuilder =
                new Step1InputMoveOutPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder,
                    step0DataBuilder);

            //build the state datas
            var rootStepData = rootDataBuilder.Create();
            var step0Data = step0DataBuilder.Create();
            var step1Data = step1DataBuilder.Create();

            var movinHouseValidationQueryResult = new MovingHouseRulesValidationResult
            {
                Output = OutputType.Passed,
                MovingHouseValidationType = MovingHouseValidationType.CanCloseAccounts
            };

            domainConfiguration.DomainFacade.QueryResolver.Current.Setup(
                x => x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(movinHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));

            //build the unit test configurator
            NewScreenTestConfigurator(domainConfiguration.DomainFacade)
                //get a runner to test events
                .NewEventTestRunner(step1Data)
                //arrange the scenario by setting the user selections in the previous steps
                .WithExistingStepData(ScreenName.PreStart, rootStepData)
                .WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)

                //act
                .WhenEvent(Step1InputMoveOutPropertyDetails.StepEvent.ToStep2)

                //assert UI
                .ThenTheResultStepIs(MovingHouseStep.Step2InputPrns)
                .ThenTheValidationPassed();

            //other domain assertions, like the domain as told to record the progress
            domainConfiguration.DomainFacade.CommandDispatcher.AssertCommandWasExecuted<RecordMovingOutProgress>();
        }

        [TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(ItHandlesEvent_To_MovingHouseValidationOnCanCloseAccountsFailed) })]
        public void ItHandlesEvent_To_MovingHouseValidationOnCanCloseAccountsFailed(AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
        {
            var domainConfiguration = accountsForMovingTypeBuilder.Create();

            //create builders that help creating reusable scenario parts
            var rootDataBuilder = new RootDataBuilder(domainConfiguration, accountsForMovingTypeBuilder.MovingType);

            var step0DataBuilder =
                new Step0OperationSelectionDataBuilder(domainConfiguration, rootDataBuilder);
            var step1DataBuilder =
                new Step1InputMoveOutPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder,
                    step0DataBuilder);

            //build the state datas
            var rootStepData = rootDataBuilder.Create();
            var step0Data = step0DataBuilder.Create();
            var step1Data = step1DataBuilder.Create();

            var movinHouseValidationQueryResult = new MovingHouseRulesValidationResult
            {
                Output = OutputType.Failed,
                MovingHouseValidationType = MovingHouseValidationType.CanCloseAccounts
            };

            //domainConfigurator.DomainFacade.QueryResolver.ExpectQuery. TODO : does not seem to be triggering IEquatable logic in query
            domainConfiguration.DomainFacade.QueryResolver.Current.Setup(
                x => x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(movinHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));

            //build the unit test configurator
            NewScreenTestConfigurator(domainConfiguration.DomainFacade)
                //get a runner to test events
                .NewEventTestRunner(step1Data)
                //arrange the scenario by setting the user selections in the previous steps
                .WithExistingStepData(ScreenName.PreStart, rootStepData)
                .WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)

                //act
                .WhenEvent(Step1InputMoveOutPropertyDetails.StepEvent.ToStep2)

                //assert UI
                .ThenTheResultStepIs(MovingHouseStep.ShowMovingHouseValidationError)
                .ThenTheValidationPassed();

            //other domain assertions, like the domain as told to record the progress
            domainConfiguration.DomainFacade.CommandDispatcher.AssertCommandWasExecuted<RecordMovingOutProgress>();
        }

        [Test]
		public void FlowIsCorrect()
		{
			Assert.AreEqual(ResidentialPortalFlowType.MovingHouse, NewScreenTestConfigurator().Adapter.GetFlowType());
		}

		[Test]
		public void ScreenStepIsCorrect()
		{
			Assert.AreEqual(MovingHouseStep.Step1InputMoveOutPropertyDetails,
				NewScreenTestConfigurator().Adapter.GetStep());
		}

		[Test]
		public void ViewPathIsCorrect()
		{
			Assert.AreEqual("Step1InputMoveOutPropertyDetails", NewScreenTestConfigurator().Adapter.GetViewPath());
		}
	}
}