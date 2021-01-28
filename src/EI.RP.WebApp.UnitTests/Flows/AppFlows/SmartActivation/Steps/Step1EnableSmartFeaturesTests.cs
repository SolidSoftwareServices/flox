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
	internal class Step1EnableSmartFeaturesTests
	{
		private FlowScreenTestConfigurator<Step1EnableSmartFeatures, ResidentialPortalFlowType> NewScreenTestConfigurator(DomainFacade domainFacade = null)
		{
			if (domainFacade == null) domainFacade = new DomainFacade();

			return new FlowScreenTestConfigurator<Step1EnableSmartFeatures, ResidentialPortalFlowType>(domainFacade.ModelsBuilder)
				//Assigns the domain stubs to the configurator to be injected in the step instances(see other methods)
				.WithStub(domainFacade.SessionProvider)
				.WithStub(domainFacade.QueryResolver)
				.WithStub(domainFacade.CommandDispatcher);
		}
			
		public async Task ItHandles_ErrorEvent()
        {
	        var domain = new AppUserConfigurator();
			var rootDataBuilder = new RootDataBuilder(domain);

	        //build the state datas
	        var rootStepData = rootDataBuilder.Create();

	        NewScreenTestConfigurator(domain.DomainFacade)
		        //get a runner to test events
		        .NewEventTestRunner(new Step1EnableSmartFeatures.ScreenModel())
		        .WithExistingStepData(ScreenName.PreStart, rootStepData)
		        .WhenEvent(ScreenEvent.ErrorOccurred)

		        //assert UI
		        .ThenTheResultStepIs(SmartActivationStep.ShowFlowGenericError);
        }

		[Test]
		public async Task ItHandles_EnableSmartActivationEvent()
		{
			var domain = new AppUserConfigurator();
			var rootDataBuilder = new RootDataBuilder(domain);

			//build the state datas
			var rootStepData = rootDataBuilder.Create();

			NewScreenTestConfigurator(domain.DomainFacade)
				//get a runner to test events
				.NewEventTestRunner(new Step1EnableSmartFeatures.ScreenModel())
				.WithExistingStepData(ScreenName.PreStart, rootStepData)
				.WhenEvent(Step1EnableSmartFeatures.StepEvent.EnableSmartServices)

				//assert UI
				.ThenTheResultStepIs(SmartActivationStep.Step2SelectPlan);
		}
	}
}