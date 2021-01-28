using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.UI.TestServices.Flows.FlowScreenUnitTest;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps;
using EI.RP.WebApp.UnitTests.Flows.AppFlows.SmartActivation.Infrastructure;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.SmartActivation.Steps
{
	[TestFixture]
	internal class ConfirmationTests
	{
		private FlowScreenTestConfigurator<Confirmation, ResidentialPortalFlowType> NewScreenTestConfigurator(DomainFacade domainFacade = null)
		{
			if (domainFacade == null) domainFacade = new DomainFacade();

			return new FlowScreenTestConfigurator<Confirmation, ResidentialPortalFlowType>(domainFacade.ModelsBuilder)
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
			var confirmationDataBuilder = new ConfirmationDataBuilder(domain);

			//build the state datas
			var rootStepData = rootDataBuilder.Create();
			var step1Data = step1DataBuilder.Create();
			var step2Data = step2DataBuilder.Create(rootStepData.AccountNumber);
			var step3Data = step3DataBuilder.Create();
			var step4Data = step4DataBuilder.Create();
			var step5Data = step5DataBuilder.Create(step2Data.SelectedPlanName);
			var confirmationData = confirmationDataBuilder.Create();

			NewScreenTestConfigurator(domain.DomainFacade)
				//get a runner to test events
				.NewEventTestRunner(confirmationData)
				.WithExistingStepData(ScreenName.PreStart, rootStepData)
				.WithExistingStepData(SmartActivationStep.Step1EnableSmartFeatures, step1Data)
				.WithExistingStepData(SmartActivationStep.Step2SelectPlan, step2Data)
				.WithExistingStepData(SmartActivationStep.Step3PaymentDetails, step3Data)
				.WithExistingStepData(SmartActivationStep.Step4BillingFrequency, step4Data)
				.WithExistingStepData(SmartActivationStep.Step5Summary, step5Data)
				.WhenEvent(ScreenEvent.ErrorOccurred)

				//assert UI
				.ThenTheResultStepIs(SmartActivationStep.ShowFlowGenericError);
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
			var step5DataBuilder = new Step5SummaryDataBuilder(domain);

			//build the state datas
			var rootStepData = rootDataBuilder.Create();
			var step1Data = step1DataBuilder.Create();
			var step2Data = step2DataBuilder.Create(rootStepData.AccountNumber);
			var step3Data = step3DataBuilder.Create();
			var step4Data = step4DataBuilder.Create();
			var step5Data = step5DataBuilder.Create(step2Data.SelectedPlanName);

			NewScreenTestConfigurator(domain.DomainFacade)
				.NewTestCreateStepDataRunner()
				.WithExistingStepData(ScreenName.PreStart, rootStepData)
				.WithExistingStepData(SmartActivationStep.Step1EnableSmartFeatures, step1Data)
				.WithExistingStepData(SmartActivationStep.Step2SelectPlan, step2Data)
				.WithExistingStepData(SmartActivationStep.Step3PaymentDetails, step3Data)
				.WithExistingStepData(SmartActivationStep.Step4BillingFrequency, step4Data)
				.WithExistingStepData(SmartActivationStep.Step5Summary, step5Data)
				.WhenCreated()
				.ThenTheStepDataIs<Confirmation.ScreenModel>(actual =>
				{
					Assert.AreEqual(step2Data.SelectedPlanName, actual.SelectedPlanName);
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
			Assert.AreEqual(SmartActivationStep.Confirmation,
				NewScreenTestConfigurator().Adapter.GetStep());
		}

		[Test]
		public void ViewPathIsCorrect()
		{
			Assert.AreEqual("Confirmation", NewScreenTestConfigurator().Adapter.GetViewPath());
		}
	}
}