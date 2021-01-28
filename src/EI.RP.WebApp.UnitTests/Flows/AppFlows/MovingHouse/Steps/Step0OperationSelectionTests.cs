using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.DomainModels.MoveHouse;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.Queries.MovingHouse.Validation;
using EI.RP.UI.TestServices.Flows.FlowScreenUnitTest;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps;
using EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Infrastructure.StepsDataBuilder;
using NUnit.Framework;
using Moq;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.MovingHouse.Steps
{
	[TestFixture]
	internal class Step0OperationSelectionTests
	{
		private FlowScreenTestConfigurator<Step0OperationSelection, ResidentialPortalFlowType> NewScreenTestConfigurator(DomainFacade domainFacade = null)
		{
			if (domainFacade == null) domainFacade = new DomainFacade();

			return new FlowScreenTestConfigurator<Step0OperationSelection, ResidentialPortalFlowType>(domainFacade.ModelsBuilder)
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

        [TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(ItHandlesEvent_To_ToStep1OnAccountContractStartedBeforeTodayPassed) })]
        public void ItHandlesEvent_To_ToStep1OnAccountContractStartedBeforeTodayPassed(AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
		{
			var domainConfigurator = accountsForMovingTypeBuilder.Create();
			//create scenario state
			var rootDataBuilder = new RootDataBuilder(domainConfigurator, accountsForMovingTypeBuilder.MovingType);
			var rootStepData = rootDataBuilder.Create();
			var step0OperationSelectionData = new Step0OperationSelectionDataBuilder(domainConfigurator, rootDataBuilder).Create();

			var movinHouseValidationQueryResult = new MovingHouseRulesValidationResult
			{
                Output = OutputType.Passed,
				MovingHouseValidationType = MovingHouseValidationType.AccountContractStartedBeforeToday
			};

            //domainConfigurator.DomainFacade.QueryResolver.ExpectQuery. TODO : does not seem to be triggering IEquatable logic in query
            domainConfigurator.DomainFacade.QueryResolver.Current.Setup(
				x => x.FetchAsync<MovingHouseValidationQuery, MovingHouseRulesValidationResult>(It.IsAny<MovingHouseValidationQuery>(), It.IsAny<bool>()))
				.Returns(Task.FromResult(movinHouseValidationQueryResult.ToOneItemArray().AsEnumerable()));

			//create the test configurator
			NewScreenTestConfigurator(domainConfigurator.DomainFacade)
			//get a runner to test the step creation
			.NewEventTestRunner(step0OperationSelectionData)
			//arrange the scenario preconditions
			.WithExistingStepData(ScreenName.PreStart, rootStepData)
			//act
			.WhenEvent(Step0OperationSelection.StepEvent.MoveElectricitySelected)
			.ThenTheResultStepIs(MovingHouseStep.Step1InputMoveOutPropertyDetails);
		}

		[TestCaseSource(nameof(MovingHouseTestCases), new object[] { nameof(ItHandles_ErrorEvent) })]
		public async Task ItHandles_ErrorEvent(AccountsForMovingTypeBuilder accountsForMovingTypeBuilder)
        {
	        var domainConfigurator = accountsForMovingTypeBuilder.Create();
	        var rootDataBuilder = new RootDataBuilder(domainConfigurator, accountsForMovingTypeBuilder.MovingType);

	        var step0OperationSelectionDataBuilder = new Step0OperationSelectionDataBuilder(domainConfigurator, rootDataBuilder);

	        //build the state datas
	        var rootStepData = rootDataBuilder.Create();
	        var step0Data = step0OperationSelectionDataBuilder.Create();

	        NewScreenTestConfigurator(domainConfigurator.DomainFacade)
		        //get a runner to test events
		        .NewEventTestRunner(step0Data)
		        .WithExistingStepData(ScreenName.PreStart, rootStepData)
		        .WhenEvent(ScreenEvent.ErrorOccurred)

		        //assert UI
		        .ThenTheResultStepIs(MovingHouseStep.ShowMovingHouseUnhandledError);
        }
	}
}