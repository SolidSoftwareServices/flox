using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.UI.TestServices.Flows.FlowScreenUnitTest;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.Steps;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.MovingHouse.Steps;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps;
using EI.RP.WebApp.UnitTests.Flows.AppFlows.SmartActivation.Infrastructure;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.SmartActivation.Steps
{
	[TestFixture]
	internal class Step3PaymentDetailsTests
	{
		private FlowScreenTestConfigurator<Step3PaymentDetails, ResidentialPortalFlowType> NewScreenTestConfigurator(DomainFacade domainFacade = null)
		{
			if (domainFacade == null) domainFacade = new DomainFacade();

			return new FlowScreenTestConfigurator<Step3PaymentDetails, ResidentialPortalFlowType>(domainFacade.ModelsBuilder)
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

	        //build the state datas
	        var rootStepData = rootDataBuilder.Create();
	        var step1Data = step1DataBuilder.Create();
	        var step2Data = step2DataBuilder.Create(rootStepData.AccountNumber);
	        var step3Data = step3DataBuilder.Create();
	       

			NewScreenTestConfigurator(domain.DomainFacade)
				//get a runner to test events
				.NewEventTestRunner(step3Data)
				.WithExistingStepData(ScreenName.PreStart, rootStepData)
				.WithExistingStepData(SmartActivationStep.Step1EnableSmartFeatures, step1Data)
				.WithExistingStepData(SmartActivationStep.Step2SelectPlan, step2Data)
				.WhenEvent(ScreenEvent.ErrorOccurred)

		        //assert UI
		        .ThenTheResultStepIs(SmartActivationStep.ShowFlowGenericError);
        }

		[Test]
		public async Task ItHandles_PaymentConfiguration()
		{
			var domain = new AppUserConfigurator();
			var rootDataBuilder = new RootDataBuilder(domain);
			var step1DataBuilder = new Step1EnableSmartFeaturesDataBuilder(domain);
			var step2DataBuilder = new Step2SelectPlanDataBuilder(domain);
			var step3DataBuilder = new Step3PaymentDetailsDataBuilder(domain);

			//build the state datas
			var rootStepData = rootDataBuilder.Create();
			var step1Data = step1DataBuilder.Create();
			var step2Data = step2DataBuilder.Create(rootStepData.AccountNumber);
			var step3Data = step3DataBuilder.Create();

			NewScreenTestConfigurator(domain.DomainFacade)
				//get a runner to test events
				.NewEventTestRunner(step3Data)
				.WithExistingStepData(ScreenName.PreStart, rootStepData)
				.WithExistingStepData(SmartActivationStep.Step1EnableSmartFeatures, step1Data)
				.WithExistingStepData(SmartActivationStep.Step2SelectPlan, step2Data)
				.WhenEvent(Step3PaymentDetails.StepEvent.AccountsPaymentConfigurationCompleted)

				//assert UI
				.ThenTheResultStepIs(SmartActivationStep.Step4BillingFrequency);
		}

		[Test]
		public void FlowIsCorrect()
		{
			Assert.AreEqual(ResidentialPortalFlowType.SmartActivation, NewScreenTestConfigurator().Adapter.GetFlowType());
		}

		[Test]
		public void ScreenStepIsCorrect()
		{
			Assert.AreEqual(SmartActivationStep.Step3PaymentDetails,
				NewScreenTestConfigurator().Adapter.GetStep());
		}

		[Test]
		public void ViewPathIsCorrect()
		{
			Assert.AreEqual("Step3PaymentDetails", NewScreenTestConfigurator().Adapter.GetViewPath());
		}
	}
}