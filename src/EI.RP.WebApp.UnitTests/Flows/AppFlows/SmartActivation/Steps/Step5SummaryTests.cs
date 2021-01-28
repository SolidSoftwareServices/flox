using System.Linq;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.UI.TestServices.Flows.FlowScreenUnitTest;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps;
using EI.RP.WebApp.UnitTests.Flows.AppFlows.SmartActivation.Infrastructure;
using EI.RP.DomainServices.Commands.SmartActivation.ActivateSmartMeter;
using EI.RP.DomainServices.Queries.SmartActivation.SmartActivationPlan;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.SmartActivation.Steps
{
	[TestFixture]
	internal class Step5SummaryTests
	{
		private FlowScreenTestConfigurator<Step5Summary, ResidentialPortalFlowType> NewScreenTestConfigurator(DomainFacade domainFacade = null)
		{
			if (domainFacade == null) domainFacade = new DomainFacade();

			return new FlowScreenTestConfigurator<Step5Summary, ResidentialPortalFlowType>(domainFacade.ModelsBuilder)
				//Assigns the domain stubs to the configurator to be injected in the step instances(see other methods)
				.WithStub(domainFacade.SessionProvider)
				.WithStub(domainFacade.QueryResolver)
				.WithStub(domainFacade.CommandDispatcher);
		}

		[Test]
		public async Task ItHandles_ErrorEvent()
		{
			var domain = new AppUserConfigurator();
			var rootDataBuilder = new RootDataBuilder(domain);
			var step1DataBuilder = new Step1EnableSmartFeaturesDataBuilder(domain);
			var step2DataBuilder = new Step2SelectPlanDataBuilder(domain);
			var step3DataBuilder = new Step3PaymentDetailsDataBuilder(domain);
			var step4DataBuilder = new Step4BillingFrequencyDataBuilder(domain);
			var step5DataBuilder = new Step5SummaryDataBuilder(domain);

			//build the state datas
			var rootStepData = rootDataBuilder.Create();
			var step1Data = step1DataBuilder.Create();
			var step2Data = step2DataBuilder.Create(rootStepData.AccountNumber);
			var step3Data = step3DataBuilder.Create();
			var step4Data = step4DataBuilder.Create();
			var step5Data = step5DataBuilder.Create(step2Data.SelectedPlanName);

			NewScreenTestConfigurator(domain.DomainFacade)
				//get a runner to test events
				.NewEventTestRunner(step5Data)
				.WithExistingStepData(ScreenName.PreStart, rootStepData)
				.WithExistingStepData(SmartActivationStep.Step1EnableSmartFeatures, step1Data)
				.WithExistingStepData(SmartActivationStep.Step2SelectPlan, step2Data)
				.WithExistingStepData(SmartActivationStep.Step3PaymentDetails, step3Data)
				.WithExistingStepData(SmartActivationStep.Step4BillingFrequency, step4Data)
				.WhenEvent(ScreenEvent.ErrorOccurred)

				//assert UI
				.ThenTheResultStepIs(SmartActivationStep.ShowFlowGenericError);
		}

		[Test]
		public async Task ItHandles_Confirmation()
		{
			var domain = new AppUserConfigurator();
			var rootDataBuilder = new RootDataBuilder(domain);
			var step1DataBuilder = new Step1EnableSmartFeaturesDataBuilder(domain);
			var step2DataBuilder = new Step2SelectPlanDataBuilder(domain);
			var step3DataBuilder = new Step3PaymentDetailsDataBuilder(domain);
			var step4DataBuilder = new Step4BillingFrequencyDataBuilder(domain);
			var step5DataBuilder = new Step5SummaryDataBuilder(domain);

			//build the state datas
			var rootStepData = rootDataBuilder.Create();
			var step1Data = step1DataBuilder.Create();
			var step2Data = step2DataBuilder.Create(rootStepData.AccountNumber);
			var step3Data = step3DataBuilder.Create();
			var step4Data = step4DataBuilder.Create();
			var step5Data = step5DataBuilder.Create(step2Data.SelectedPlanName);

		

			NewScreenTestConfigurator(domain.DomainFacade)
				//get a runner to test events
				.NewEventTestRunner(step5Data)
				.WithExistingStepData(ScreenName.PreStart, rootStepData)
				.WithExistingStepData(SmartActivationStep.Step1EnableSmartFeatures, step1Data)
				.WithExistingStepData(SmartActivationStep.Step2SelectPlan, step2Data)
				.WithExistingStepData(SmartActivationStep.Step3PaymentDetails, step3Data)
				.WithExistingStepData(SmartActivationStep.Step4BillingFrequency, step4Data)
				.WhenEvent(Step5Summary.StepEvent.RequestCompleteSmartActivation)
				//assert UI
				.ThenTheResultStepIs(SmartActivationStep.Confirmation);

			var cmd = new ActivateSmartMeterCommand(step5DataBuilder.Mprn,
													step5Data.AccountNumber,
													step2DataBuilder.SelectedPlan,
													step5Data.SelectedFreeDay,
													step5Data.MonthlyBilling,
													step5Data.MonthlyBillingSelectedDay,
													step5DataBuilder.DebitDomainCommands);

			domain.DomainFacade.CommandDispatcher.AssertCommandWasExecuted(cmd);
		}

		[Test]
		public async Task ItHasCorrectScreenModelInfoData()
		{
			var domain = new AppUserConfigurator();
			var rootDataBuilder = new RootDataBuilder(domain);
			var step1DataBuilder = new Step1EnableSmartFeaturesDataBuilder(domain);
			var step2DataBuilder = new Step2SelectPlanDataBuilder(domain);
			var step3DataBuilder = new Step3PaymentDetailsDataBuilder(domain);
			var step4DataBuilder = new Step4BillingFrequencyDataBuilder(domain);

			//build the state datas
			var rootStepData = rootDataBuilder.Create();
			var step1Data = step1DataBuilder.Create();
			var step2Data = step2DataBuilder.Create(rootStepData.AccountNumber);
			rootStepData.SelectedPlanName = step2Data.SelectedPlanName;
			var step3Data = step3DataBuilder.Create();
			var step4Data = step4DataBuilder.Create();

			var paymentResult = step3Data.CalledFlowResult;
			var paymentInfo = paymentResult.ConfigurationSelectionResults;

			var isAlternativePayer = paymentInfo.Any(c =>
					 c.SelectedPaymentSetUpType == PaymentSetUpType.AlternativePayer);

			var isDirectDebit = paymentInfo.Any(c =>
								 c.SelectedPaymentSetUpType == PaymentSetUpType.SetUpNewDirectDebit ||
								 c.SelectedPaymentSetUpType == PaymentSetUpType.UseExistingDirectDebit);

			var paymentMethod = isAlternativePayer ? PaymentMethodType.DirectDebitNotAvailable : 
								isDirectDebit ? PaymentMethodType.DirectDebit : PaymentMethodType.Manual;

			var isMonthlyBilling = step4Data.BillingFrequencyType.GetValueOrDefault() == BillingFrequencyType.EveryMonth;
			var monthlyBillingSelectedDay = step4Data.BillingDayOfMonth.GetValueOrDefault();

			NewScreenTestConfigurator(domain.DomainFacade)
				.NewTestCreateStepDataRunner()
				.WithExistingStepData(ScreenName.PreStart, rootStepData)
				.WithExistingStepData(SmartActivationStep.Step1EnableSmartFeatures, step1Data)
				.WithExistingStepData(SmartActivationStep.Step2SelectPlan, step2Data)
				.WithExistingStepData(SmartActivationStep.Step3PaymentDetails, step3Data)
				.WithExistingStepData(SmartActivationStep.Step4BillingFrequency, step4Data)
				.WhenCreated()
				.ThenTheStepDataIs<Step5Summary.ScreenModel>(actual =>
				{
					Assert.AreEqual(rootStepData.AccountNumber, actual.AccountNumber);
					Assert.AreEqual(rootStepData.SelectedPlanName, actual.SelectedPlanName);
					Assert.AreEqual(step2Data.SelectedFreeDay, actual.SelectedFreeDay);
					Assert.AreEqual(paymentInfo, actual.PaymentInfo);
					Assert.AreEqual(isAlternativePayer, actual.AlternativePayer);
					Assert.AreEqual(paymentMethod, actual.PaymentMethod);
					Assert.AreEqual(isMonthlyBilling, actual.MonthlyBilling);
					Assert.AreEqual(monthlyBillingSelectedDay, actual.MonthlyBillingSelectedDay);
				});
		}

		[Test]
		public void FlowIsCorrect()
		{
			Assert.AreEqual(ResidentialPortalFlowType.SmartActivation, NewScreenTestConfigurator().Adapter.GetFlowType());
		}

		[Test]
		public void ScreenStepIsCorrect()
		{
			Assert.AreEqual(SmartActivationStep.Step5Summary,
				NewScreenTestConfigurator().Adapter.GetStep());
		}

		[Test]
		public void ViewPathIsCorrect()
		{
			Assert.AreEqual("Step5Summary", NewScreenTestConfigurator().Adapter.GetViewPath());
		}
	}
}