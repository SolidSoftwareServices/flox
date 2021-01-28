using System.Collections.Generic;
using System.Linq;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.UI.TestServices.Flows.FlowScreenUnitTest;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps;
using EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Infrastructure.StepsDataBuilder;
using NUnit.Framework;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.CoreServices.System;
using Moq;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Steps
{
	[TestFixture]
	internal class Step4ConfigurePaymentTests
	{
		private FlowScreenTestConfigurator<Step4ConfigurePayment, ResidentialPortalFlowType> NewScreenTestConfigurator(DomainFacade domainFacade = null)
		{
			if (domainFacade == null) domainFacade = new DomainFacade();

			return new FlowScreenTestConfigurator<Step4ConfigurePayment, ResidentialPortalFlowType>(domainFacade.ModelsBuilder)
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

			var step0DataBuilder = new Step0OperationSelectionDataBuilder(domainConfiguration, rootDataBuilder);
			var step1DataBuilder = new Step1InputMoveOutPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder, step0DataBuilder);
			var step2DataBuilder = new Step2InputPrnsDataBuilder(domainConfiguration, rootDataBuilder, step0DataBuilder, step1DataBuilder);
			var step2ConfirmDataBuilder = new Step2ConfirmAddressDataBuilder(domainConfiguration, rootDataBuilder);
			var step3DataBuilder = new Step3InputMoveInPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder);
			var step4DataBuilder = new Step4ConfigurePaymentDataBuilder(domainConfiguration, rootDataBuilder);

			var rootStepData = rootDataBuilder.Create();
			var step0Data = step0DataBuilder.Create();
			var step1Data = step1DataBuilder.Create();
			var step2Data = step2DataBuilder.Create();
			var step2ConfirmData = step2ConfirmDataBuilder.Create();
			var step3Data = step3DataBuilder.Create();
			var step4Data = step4DataBuilder.Create();


			NewScreenTestConfigurator(domainConfiguration.DomainFacade)
		        //get a runner to test events
		        .NewEventTestRunner(step4Data)
		        //arrange the scenario by setting the user selections in the previous steps
		        .WithExistingStepData(ScreenName.PreStart, rootStepData)
		        .WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)
		        .WithExistingStepData(MovingHouseStep.Step1InputMoveOutPropertyDetails, step1Data)
		        .WithExistingStepData(MovingHouseStep.Step2InputPrns, step2Data)
		        .WithExistingStepData(MovingHouseStep.Step2ConfirmAddress, step2ConfirmData)
		        .WithExistingStepData(MovingHouseStep.Step3InputMoveInPropertyDetails, step3Data)

				.WhenEvent(ScreenEvent.ErrorOccurred)
		        //assert UI
		        .ThenTheResultStepIs(MovingHouseStep.ShowMovingHouseUnhandledError);
        }

        [TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(ItHandlesEvent_To_MovingHouseValidationOnIsContractSaleChecksOkPassed) })]
        public void ItHandlesEvent_To_MovingHouseValidationOnIsContractSaleChecksOkPassed(AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
        {
            var domainConfiguration = accountsForMovingTypeBuilder.Create();

            //create builders that help creating reusable scenario parts
            var rootDataBuilder = new RootDataBuilder(domainConfiguration, accountsForMovingTypeBuilder.MovingType);
            var step0DataBuilder = new Step0OperationSelectionDataBuilder(domainConfiguration, rootDataBuilder);
            var step1DataBuilder = new Step1InputMoveOutPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder, step0DataBuilder);
            var step2DataBuilder = new Step2InputPrnsDataBuilder(domainConfiguration, rootDataBuilder, step0DataBuilder, step1DataBuilder);
            var step3DataBuilder = new Step3InputMoveInPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder);
            var step4DataBuilder = new Step4ConfigurePaymentDataBuilder(domainConfiguration, rootDataBuilder);

            var rootStepData = rootDataBuilder.Create();
            var step0Data = step0DataBuilder.Create();
            var step1Data = step1DataBuilder.Create();
            var step2Data = step2DataBuilder.Create();
            var step3Data = step3DataBuilder.Create();
            var step4Data = step4DataBuilder.Create();

            var movingHouseValidationQueryResult = new MovingHouseRulesValidationResult
            {
                Output = OutputType.Passed,
                MovingHouseValidationType = MovingHouseValidationType.IsContractSaleChecksOk
            };

            domainConfiguration.DomainFacade.QueryResolver.Current.Setup(
                x => x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(movingHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));

            //build the unit test configurator
            NewScreenTestConfigurator(domainConfiguration.DomainFacade)
                //get a runner to test events
                .NewEventTestRunner(step4Data)
                //arrange the scenario by setting the user selections in the previous steps
                .WithExistingStepData(ScreenName.PreStart, rootStepData)
                .WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)
                .WithExistingStepData(MovingHouseStep.Step1InputMoveOutPropertyDetails, step1Data)
                .WithExistingStepData(MovingHouseStep.Step2InputPrns, step2Data)
                .WithExistingStepData(MovingHouseStep.Step3InputMoveInPropertyDetails, step3Data)

                //act
                .WhenEvent(Step4ConfigurePayment.StepEvent.AccountsPaymentConfigurationCompleted)

                //assert UI
                .ThenTheResultStepIs(MovingHouseStep.Step5ReviewAndComplete)
                .ThenTheValidationPassed();
        }

        [TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(ItHandlesEvent_To_MovingHouseValidationOnIsContractSaleChecksOkFailed) })]
        public void ItHandlesEvent_To_MovingHouseValidationOnIsContractSaleChecksOkFailed(AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
        {
            var domainConfiguration = accountsForMovingTypeBuilder.Create();

            //create builders that help creating reusable scenario parts
            var rootDataBuilder = new RootDataBuilder(domainConfiguration, accountsForMovingTypeBuilder.MovingType);

            var step0DataBuilder = new Step0OperationSelectionDataBuilder(domainConfiguration, rootDataBuilder);
            var step1DataBuilder = new Step1InputMoveOutPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder, step0DataBuilder);
            var step2DataBuilder = new Step2InputPrnsDataBuilder(domainConfiguration, rootDataBuilder, step0DataBuilder, step1DataBuilder);
            var step3DataBuilder = new Step3InputMoveInPropertyDetailsDataBuilder(domainConfiguration, rootDataBuilder);
            var step4DataBuilder = new Step4ConfigurePaymentDataBuilder(domainConfiguration, rootDataBuilder);

            //build the state datas
            var rootStepData = rootDataBuilder.Create();
            var step0Data = step0DataBuilder.Create();
            var step1Data = step1DataBuilder.Create();
            var step2Data = step2DataBuilder.Create();
            var step3Data = step3DataBuilder.Create();
            var step4Data = step4DataBuilder.Create();

            var movingHouseValidationQueryResult = new MovingHouseRulesValidationResult
            {
                Output = OutputType.Failed,
                MovingHouseValidationType = MovingHouseValidationType.IsContractSaleChecksOk
            };

            domainConfiguration.DomainFacade.QueryResolver.Current.Setup(
                x => x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(movingHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));

            //build the unit test configurator
            NewScreenTestConfigurator(domainConfiguration.DomainFacade)
                //get a runner to test events
                .NewEventTestRunner(step4Data)
                //arrange the scenario by setting the user selections in the previous steps
                .WithExistingStepData(ScreenName.PreStart, rootStepData)
                .WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)
                .WithExistingStepData(MovingHouseStep.Step1InputMoveOutPropertyDetails, step1Data)
                .WithExistingStepData(MovingHouseStep.Step2InputPrns, step2Data)
                .WithExistingStepData(MovingHouseStep.Step3InputMoveInPropertyDetails, step3Data)

                //act
                .WhenEvent(Step4ConfigurePayment.StepEvent.AccountsPaymentConfigurationCompleted)

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
            Assert.AreEqual(MovingHouseStep.Step4ConfigurePayment,
                NewScreenTestConfigurator().Adapter.GetStep());
        }

        [Test]
        public void ViewPathIsCorrect()
        {
            Assert.AreEqual("Step4ConfigurePayment", NewScreenTestConfigurator().Adapter.GetViewPath());
        }
    }
}