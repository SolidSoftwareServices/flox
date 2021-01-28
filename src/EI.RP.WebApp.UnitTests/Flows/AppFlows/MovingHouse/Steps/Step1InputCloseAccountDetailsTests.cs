using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.UI.TestServices.Flows.FlowScreenUnitTest;
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
	internal class Step1InputCloseAccountDetailsTests
    {
		private FlowScreenTestConfigurator<Step1InputCloseAccountDetails, ResidentialPortalFlowType>
			NewScreenTestConfigurator(DomainFacade domainFacade = null)
		{
			//creates the domain layer facade
			if (domainFacade == null) domainFacade = new DomainFacade();
			//creates a flo test configurator
			return new FlowScreenTestConfigurator<Step1InputCloseAccountDetails, ResidentialPortalFlowType>(domainFacade.ModelsBuilder)
				//Assigns the domain stubs to the configurator to be injected in the step instances(see other methods)
				.WithStub(domainFacade.SessionProvider)
				.WithStub(domainFacade.QueryResolver)
				.WithStub(domainFacade.CommandDispatcher);
		}

        private static IEnumerable<TestCaseData> MovingHouseTestCases(string caseName)
        {
            var movingHouseTypes = MovingHouseType.AllValues.Where(x =>
                    x.IsOneOf(MovingHouseType.CloseElectricityAndGas, MovingHouseType.CloseElectricity,
                        MovingHouseType.CloseGas))
                .Cast<MovingHouseType>();

            foreach (var movingType in movingHouseTypes)
            {
                //compose the test case
                yield return new TestCaseData(new AccountsForMovingTypeBuilder(movingType))
                    .SetName($"{caseName}_{movingType}");
            }
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
				.ThenTheStepDataIs<Step1InputCloseAccountDetails.ScreenModel>(actual =>
				{
					var expected = ShowTermsAcknowledgmentType.None;
					if (step0OperationSelectionData.MovingHouseType.IsOneOf(MovingHouseType.CloseElectricityAndGas))
						expected = ShowTermsAcknowledgmentType.ShowBoth;

					if (step0OperationSelectionData.MovingHouseType.IsOneOf(MovingHouseType.CloseGas))
						expected = ShowTermsAcknowledgmentType.ShowGas;

					if (step0OperationSelectionData.MovingHouseType.IsOneOf(MovingHouseType.CloseElectricity))
						expected = ShowTermsAcknowledgmentType.ShowElectricity;

					Assert.AreEqual(expected,
						actual.ShowTermsAcknowledgmentType);
				});
		}

        private ScreenEvent GetNextStepEvent(MovingHouseType movingHouseType)
        {
            if (movingHouseType == MovingHouseType.CloseGas) return Step1InputCloseAccountDetails.StepEvent.CloseGas;
            if (movingHouseType == MovingHouseType.CloseElectricityAndGas) return Step1InputCloseAccountDetails.StepEvent.CloseElectricityAndGas;

            return Step1InputCloseAccountDetails.StepEvent.CloseElectricity;
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
                new Step1InputCloseAccountDetailsDataBuilder(domainConfiguration, rootDataBuilder,
                    step0OperationSelectionDataBuilder);

            //build the state datas
            var rootStepData = rootDataBuilder.Create();
            var step0Data = step0OperationSelectionDataBuilder.Create();
            var step1Data = step1DataBuilder.Create();

            var movingHouseValidationQueryResult = new MovingHouseRulesValidationResult
            {
                Output = OutputType.Failed,
                MovingHouseValidationType = MovingHouseValidationType.PhoneNumberIsValid
            };

            domainConfiguration.DomainFacade.QueryResolver.Current.Setup(
                    x => x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(movingHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));

            var stepEvent = GetNextStepEvent(accountsForMovingTypeBuilder.MovingType);

            //build the unit test configurator
            NewScreenTestConfigurator(domainConfiguration.DomainFacade)
                //get a runner to test events
                .NewEventTestRunner(step1Data)
                //arrange the scenario by setting the user selections in the previous steps
                .WithExistingStepData(ScreenName.PreStart, rootStepData)
                .WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)

                //act
                .WhenEvent(stepEvent)

                //assert UI
                .ThenTheResultStepIs(MovingHouseStep.ShowMovingHouseValidationError)
                .ThenTheValidationPassed();

        }

        [TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(ItHandlesEvent_To_MovingHouseValidationOnCanCloseAccountsFailed) })]
        public void ItHandlesEvent_To_MovingHouseValidationOnCanCloseAccountsFailed(AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
        {
            var domainConfiguration = accountsForMovingTypeBuilder.Create();

            //create builders that help creating reusable scenario parts
            var rootDataBuilder = new RootDataBuilder(domainConfiguration, accountsForMovingTypeBuilder.MovingType);
            var step0OperationSelectionDataBuilder =
                new Step0OperationSelectionDataBuilder(domainConfiguration, rootDataBuilder);
            var step1DataBuilder =
                new Step1InputCloseAccountDetailsDataBuilder(domainConfiguration, rootDataBuilder,
                    step0OperationSelectionDataBuilder);

            //build the state datas
            var rootStepData = rootDataBuilder.Create();
            var step0Data = step0OperationSelectionDataBuilder.Create();
            var step1Data = step1DataBuilder.Create();

            var movingHouseValidationQueryResult = new MovingHouseRulesValidationResult
            {
                Output = OutputType.Failed,
                MovingHouseValidationType = MovingHouseValidationType.CanCloseAccounts
            };

            domainConfiguration.DomainFacade.QueryResolver.Current.Setup(
                    x => x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(movingHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));

            var stepEvent = GetNextStepEvent(accountsForMovingTypeBuilder.MovingType);

            //build the unit test configurator
            NewScreenTestConfigurator(domainConfiguration.DomainFacade)
                //get a runner to test events
                .NewEventTestRunner(step1Data)
                //arrange the scenario by setting the user selections in the previous steps
                .WithExistingStepData(ScreenName.PreStart, rootStepData)
                .WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)

                //act
                .WhenEvent(stepEvent)

                //assert UI
                .ThenTheResultStepIs(MovingHouseStep.ShowMovingHouseValidationError)
                .ThenTheValidationPassed();
        }

		[TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(ItHandlesEvent_ConfirmCloseAccount) })]
		public void ItHandlesEvent_ConfirmCloseAccount(AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
		{
			var domainConfiguration = accountsForMovingTypeBuilder.Create();

			//create builders that help creating reusable scenario parts
			var rootDataBuilder = new RootDataBuilder(domainConfiguration, accountsForMovingTypeBuilder.MovingType);
			var step0OperationSelectionDataBuilder = new Step0OperationSelectionDataBuilder(domainConfiguration, rootDataBuilder);
			var step1DataBuilder = new Step1InputCloseAccountDetailsDataBuilder(domainConfiguration, rootDataBuilder, step0OperationSelectionDataBuilder);

			var rootStepData = rootDataBuilder.Create();
			var step0Data = step0OperationSelectionDataBuilder.Create();
			var step1Data = step1DataBuilder.Create();

			step1Data.UserMeterInputFields.MeterReadingDay = "123";
			step1Data.UserMeterInputFields.MeterReading24Hrs = "123";
			step1Data.UserMeterInputFields.MeterReadingGas = "123";
			step1Data.UserMeterInputFields.MeterReadingNight = "123";
			step1Data.UserMeterInputFields.MeterReadingNightStorageHeater = "123";

			var movingHouseValidationQueryResult = new MovingHouseRulesValidationResult
			{
				Output = OutputType.Passed, 
				MovingHouseValidationType = MovingHouseValidationType.HasElectricNewPremiseAccountDetailsInSwitch
			};

			ScreenEvent stepEvent = null;

			if(accountsForMovingTypeBuilder.MovingType == MovingHouseType.CloseElectricityAndGas)
			{
				stepEvent = Step1InputCloseAccountDetails.StepEvent.CloseElectricityAndGas;
			}
			else if (accountsForMovingTypeBuilder.MovingType == MovingHouseType.CloseElectricity)
			{
				stepEvent = Step1InputCloseAccountDetails.StepEvent.CloseElectricity;
			}
			else if (accountsForMovingTypeBuilder.MovingType == MovingHouseType.CloseGas)
			{
				stepEvent = Step1InputCloseAccountDetails.StepEvent.CloseGas;
			}
			
			domainConfiguration.DomainFacade.QueryResolver.Current.Setup(
				x => x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
				.Returns(Task.FromResult(movingHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));

			//build the unit test configurator
			NewScreenTestConfigurator(domainConfiguration.DomainFacade)
				//get a runner to test events
				.NewEventTestRunner(step1Data)
				//arrange the scenario by setting the user selections in the previous steps
				.WithExistingStepData(ScreenName.PreStart, rootStepData)
				.WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)

				//act
			.WhenEvent(stepEvent)

			//assert UI
			.ThenTheResultStepIs(MovingHouseStep.CloseAccountConfirmation)
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
			Assert.AreEqual(MovingHouseStep.StepCloseAccounts,
				NewScreenTestConfigurator().Adapter.GetStep());
		}

		[Test]
		public void ViewPathIsCorrect()
		{
			Assert.AreEqual("Step1InputCloseAccountDetails", NewScreenTestConfigurator().Adapter.GetViewPath());
		}
	}
}