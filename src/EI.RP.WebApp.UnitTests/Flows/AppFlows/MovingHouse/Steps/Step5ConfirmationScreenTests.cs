using System.Collections.Generic;
using System.Linq;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.CoreServices.System;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.UI.TestServices.Flows.FlowScreenUnitTest;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps;
using EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Infrastructure.StepsDataBuilder;
using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using System;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Steps
{
	[TestFixture]
	internal class Step5ConfirmationScreenTests
	{
		private FlowScreenTestConfigurator<Step5ConfirmationScreen, ResidentialPortalFlowType> NewScreenTestConfigurator(DomainFacade domainFacade = null)
		{
			if (domainFacade == null) domainFacade = new DomainFacade();

			return new FlowScreenTestConfigurator<Step5ConfirmationScreen, ResidentialPortalFlowType>(domainFacade.ModelsBuilder)
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
			var step5ConfirmDataBuilder = new Step5ConfirmationDataBuilder(domainConfiguration, rootDataBuilder);

			var rootStepData = rootDataBuilder.Create();
			var step0Data = step0DataBuilder.Create();
			var step1Data = step1DataBuilder.Create();
			var step2Data = step2DataBuilder.Create();
			var step2ConfirmData = step2ConfirmDataBuilder.Create();
			var step3Data = step3DataBuilder.Create();
			var step4Data = step4DataBuilder.Create();
			var step5ConfirmData = step5ConfirmDataBuilder.Create();

			NewScreenTestConfigurator(domainConfiguration.DomainFacade)
		        //get a runner to test events
		        .NewEventTestRunner(step5ConfirmData)
		        //arrange the scenario by setting the user selections in the previous steps
		        .WithExistingStepData(ScreenName.PreStart, rootStepData)
		        .WithExistingStepData(MovingHouseStep.Step0OperationSelection, step0Data)
		        .WithExistingStepData(MovingHouseStep.Step1InputMoveOutPropertyDetails, step1Data)
		        .WithExistingStepData(MovingHouseStep.Step2InputPrns, step2Data)
		        .WithExistingStepData(MovingHouseStep.Step2ConfirmAddress, step2ConfirmData)
		        .WithExistingStepData(MovingHouseStep.Step3InputMoveInPropertyDetails, step3Data)
		        .WithExistingStepData(MovingHouseStep.Step4ConfigurePayment, step4Data)

				.WhenEvent(ScreenEvent.ErrorOccurred)
		        //assert UI
		        .ThenTheResultStepIs(MovingHouseStep.ShowMovingHouseUnhandledError);
        }

        [Test]
        public void FlowIsCorrect()
        {
            Assert.AreEqual(ResidentialPortalFlowType.MovingHouse, NewScreenTestConfigurator().Adapter.GetFlowType());
        }

        [Test]
        public void ScreenStepIsCorrect()
        {
            Assert.AreEqual(MovingHouseStep.Step5ReviewConfirmation,
                NewScreenTestConfigurator().Adapter.GetStep());
        }

        [Test]
        public void ViewPathIsCorrect()
        {
            Assert.AreEqual(nameof(Step5ConfirmationScreen), NewScreenTestConfigurator().Adapter.GetViewPath());
        }
    }
}