using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.UI.TestServices.Flows.FlowInitializerUnitTest;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.BusinessPartnersSearch.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.BusinessPartnersSearch.Steps;
using NUnit.Framework;
using System.Threading.Tasks;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.BusinessPartnersSearch.Steps
{
    [TestFixture]
    internal class BusinessPartnersSearchFlowInitializerTest
    {
        private FlowInitializerTestConfigurator<FlowInitializer, ResidentialPortalFlowType> NewTestConfigurator(
            bool anonymousUser = false)
        {
            return NewTestConfigurator(anonymousUser
                ? new DomainFacade()
                : new AppUserConfigurator(ResidentialPortalUserRole.AgentUser).Execute().SetValidSession().DomainFacade);
        }

        private FlowInitializerTestConfigurator<FlowInitializer, ResidentialPortalFlowType> NewTestConfigurator(
            DomainFacade domainFacade)
        {
            if (domainFacade == null) domainFacade = new DomainFacade();

            return new FlowInitializerTestConfigurator<FlowInitializer, ResidentialPortalFlowType>(domainFacade.ModelsBuilder)
                //Assigns the domain stubs to the configurator to be injected in the step instances(see other methods)
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
            Assert.AreEqual(ResidentialPortalFlowType.BusinessPartnersSearch, NewTestConfigurator().Adapter.GetFlowType());
        }
        
        [Test]
        public async Task ItHandles_StartEvent()
        {
            NewTestConfigurator()
                .NewEventTestRunner()
                .WhenEvent(ScreenEvent.Start)
                .TheResultStepIs(BusinessPartnersSearchStep.SearchAndShowResultsStep);
        }

        [Test, Theory]
        public async Task Can_InitializeContext()
        {
            NewTestConfigurator()
                .NewInitializationRunner()
                .Run()
                .AssertTriggeredEventIs(actual => Assert.AreEqual(ScreenEvent.Start, actual));
        }

    }
}

