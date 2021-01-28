using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.UI.TestServices.Flows.FlowInitializerUnitTest;
using EI.RP.UI.TestServices.Flows.FlowScreenUnitTest;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Flows.AppFlows.ProductAndServices.FlowDefinitions;
using EI.RP.WebApp.Flows.AppFlows.ProductAndServices.Steps;
using EI.RP.WebApp.Infrastructure.Settings;
using NUnit.Framework;
using System.Threading.Tasks;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.ProductAndServices.Steps
{
    [TestFixture]
    internal class FlowInitializerTests
    {
        private FlowInitializerTestConfigurator<FlowInitializer, ResidentialPortalFlowType> NewTestConfigurator(
            bool anonymousUser = false)
        {
            return NewTestConfigurator(anonymousUser
                ? new DomainFacade()
                : new AppUserConfigurator().Execute().SetValidSession().DomainFacade);
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
            Assert.AreEqual(ResidentialPortalFlowType.ProductAndServices, NewTestConfigurator().Adapter.GetFlowType());
        }

        private static AppUserConfigurator ConfigureDomain(bool mainAccountIsElectricityAccount, bool withDuelFuel)
        {
            var cfg = new AppUserConfigurator(new DomainFacade())
                .SetValidSession();
            var mainAccount = mainAccountIsElectricityAccount
                ? (CommonElectricityAndGasAccountConfigurator)cfg.AddElectricityAccount()
                : cfg.AddGasAccount();
            if (withDuelFuel)
            {
                if (mainAccount.AccountType == ClientAccountType.Electricity)
                    cfg.AddGasAccount(duelFuelSisterAccount: cfg.ElectricityAccount());
                else
                    cfg.AddElectricityAccount(duelFuelSisterAccount: cfg.GasAccount());
            }

            cfg.Execute();
            return cfg;
        }

        [Test]
        public async Task ItHandles_StartEvent()
        {
            NewTestConfigurator()
                .NewEventTestRunner()
                .WhenEvent(ScreenEvent.Start)
                .TheResultStepIs(ProductAndServicesStep.ProductAndServices);
        }

        [Test, Theory]
        public async Task Can_InitializeContext(bool mainAccountIsElectricityAccount, bool withDuelFuel)
        {
            var cfg = ConfigureDomain(mainAccountIsElectricityAccount, withDuelFuel);

            NewTestConfigurator(cfg.DomainFacade)
                .NewInitializationRunner()
                .Run()
                .AssertTriggeredEventIs(actual => Assert.AreEqual(ScreenEvent.Start, actual));
        }

    }
}
