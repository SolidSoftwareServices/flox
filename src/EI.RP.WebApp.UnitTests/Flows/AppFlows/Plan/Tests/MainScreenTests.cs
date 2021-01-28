using System.Collections.Generic;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainServices.Commands.Billing.ChangePaperBillChoice;
using EI.RP.DomainServices.Commands.Contracts.ChangeSmartPlanToStandard;
using EI.RP.UI.TestServices.Flows.FlowScreenUnitTest;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.Plan.Steps;
using EI.RP.WebApp.UnitTests.Flows.AppFlows.Plan.Infrastructure.Builders;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.Plan.Tests
{
	[TestFixture]
	internal class MainScreenTests
	{
		private FlowScreenTestConfigurator<MainScreen, ResidentialPortalFlowType> NewScreenTestConfigurator(
			DomainFacade domainFacade = null)
		{
			if (domainFacade == null) domainFacade = new DomainFacade();

			return new FlowScreenTestConfigurator<MainScreen, ResidentialPortalFlowType>(domainFacade.ModelsBuilder)
				//Assigns the domain stubs to the configurator to be injected in the step instances(see other methods)
				.WithStub(domainFacade.SessionProvider)
				.WithStub(domainFacade.QueryResolver)
				.WithStub(domainFacade.CommandDispatcher);
		}

		private static IEnumerable<TestCaseData> ItCanHandlePaperBillSelectionEventsTestCases()
		{
			yield return new TestCaseData(MainScreen.StepEvent.SwitchOnPaperBill, PaperBillChoice.On);
			yield return new TestCaseData(MainScreen.StepEvent.SwitchOffPaperBill, PaperBillChoice.Off);
		}

		[TestCaseSource(nameof(ItCanHandlePaperBillSelectionEventsTestCases))]
		public void ItCanHandlePaperBillSelectionEvents(ScreenEvent triggeredEvent, PaperBillChoice expectedChoice)
		{
			var domainConfigurator = new DomainConfigurator().Create();
			//create scenario state
			var rootDataBuilder = new RootDataBuilder(domainConfigurator);
			var rootStepData = rootDataBuilder.Create();

			var stepData = new PlanDetailsDataBuilder(domainConfigurator, rootDataBuilder).Create();


			//create the test configurator
			NewScreenTestConfigurator(domainConfigurator.DomainFacade)
				//get a runner to test the step creation
				.NewEventTestRunner(stepData)
				//arrange the scenario preconditions
				.WithExistingStepData(ScreenName.PreStart, rootStepData)
				//act
				.WhenEvent(triggeredEvent);

			//assert command
			domainConfigurator.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(
				new ChangePaperBillChoiceCommand(rootDataBuilder.AccountNumber, expectedChoice));
		}

		private static IEnumerable<TestCaseData> ItCanHandleSmartSwitchOffMeterDataEventsTestCases()
		{
			yield return new TestCaseData(MainScreen.StepEvent.SwitchOffMeterData);
		}

		[TestCaseSource(nameof(ItCanHandleSmartSwitchOffMeterDataEventsTestCases))]
		public void ItCanHandleSmartSwitchOffMeterDataEvents(ScreenEvent triggeredEvent)
		{
			var domainConfigurator = new DomainConfigurator().Create();
			//create scenario state
			var rootDataBuilder = new RootDataBuilder(domainConfigurator);
			var rootStepData = rootDataBuilder.Create();

			var stepData = new PlanDetailsDataBuilder(domainConfigurator, rootDataBuilder).Create();


			//create the test configurator
			NewScreenTestConfigurator(domainConfigurator.DomainFacade)
				//get a runner to test the step creation
				.NewEventTestRunner(stepData)
				//arrange the scenario preconditions
				.WithExistingStepData(ScreenName.PreStart, rootStepData)
				//act
				.WhenEvent(triggeredEvent)
				.ThenTheStepDataAfterIs<MainScreen.ScreenModel>(actual =>
				{
					Assert.IsTrue(actual.IsMoveToStandardPlanRequestSendSuccesfully);
				});

			//assert command
			domainConfigurator.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(
				new ChangeSmartPlanToStandardCommand(rootDataBuilder.AccountNumber));
		}
	}
}