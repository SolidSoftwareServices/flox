using System.Threading.Tasks;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.UI.TestServices.Flows.FlowInitializerUnitTest;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.SmartActivation.Steps;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.SmartActivation.Steps
{
	[TestFixture]
	internal class SmartActivationFlowInitializerTests
	{
		private FlowInitializerTestConfigurator<SmartActivationFlowInitializer, ResidentialPortalFlowType> NewTestConfigurator(
			bool anonymousUser = false)
		{
			return NewTestConfigurator(anonymousUser
				? new DomainFacade()
				: new AppUserConfigurator().Execute().SetValidSession().DomainFacade);
		}

		private FlowInitializerTestConfigurator<SmartActivationFlowInitializer, ResidentialPortalFlowType> NewTestConfigurator(
			DomainFacade domainFacade)
		{
			if (domainFacade == null) domainFacade = new DomainFacade();

			return new FlowInitializerTestConfigurator<SmartActivationFlowInitializer, ResidentialPortalFlowType>(
					domainFacade.ModelsBuilder)
				.WithStub(domainFacade.SessionProvider)
				.WithStub(domainFacade.QueryResolver)
				.WithStub(domainFacade.CommandDispatcher);
		}

		[Test]
		[Theory]
		public async Task CanAuthorize(bool anonymousUser)
		{
			var expected = !anonymousUser;
			var actual = NewTestConfigurator(anonymousUser).Adapter.Authorize(false);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void FlowTypeIsCorrect()
		{
			Assert.AreEqual(ResidentialPortalFlowType.SmartActivation, NewTestConfigurator().Adapter.GetFlowType());
		}

		[Test]
		public async Task ItHandles_ErrorEvent()
		{
			NewTestConfigurator()
				.NewEventTestRunner()
				.WhenEvent(ScreenEvent.ErrorOccurred)
				.TheResultStepIs(SmartActivationStep.ShowFlowGenericError);
		}

		[Theory]
		public async Task ItHandles_StartEvent(bool isSmartAndEligible)
		{
			var rootStepData = new SmartActivationFlowInitializer.StepsSharedData()
			{
				IsSmartAndEligible = isSmartAndEligible
			};

			NewTestConfigurator()
				.NewEventTestRunner()
				.GivenTheStepDataIs(rootStepData)
				.WhenEvent(ScreenEvent.Start)
				.TheResultStepIs((step) =>
				{
					if (isSmartAndEligible)
					{
						Assert.AreEqual(SmartActivationStep.Step1EnableSmartFeatures, step);
					}
					else
					{
						Assert.AreEqual(SmartActivationStep.ShowFlowGenericError, step);
					}
				});
		}
	}
}