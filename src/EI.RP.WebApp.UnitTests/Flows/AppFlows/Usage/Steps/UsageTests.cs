using System.Threading.Tasks;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.UI.TestServices.Flows.FlowScreenUnitTest;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Infrastructure.Settings;
using EI.RP.WebApp.UnitTests.Flows.AppFlows.Usage.Infrastructure.StepsDataBuilder;
using NUnit.Framework;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.Usage.Steps
{
    [TestFixture]
    public class UsageTests
    {
        private FlowScreenTestConfigurator<WebApp.Flows.AppFlows.Usage.Steps.Usage, ResidentialPortalFlowType> NewScreenTestConfigurator(
            bool anonymousUser = false)
        {
            return NewScreenTestConfigurator(anonymousUser
                ? new DomainFacade()
                : new AppUserConfigurator().Execute().SetValidSession().DomainFacade);
        }

        private FlowScreenTestConfigurator<WebApp.Flows.AppFlows.Usage.Steps.Usage, ResidentialPortalFlowType> NewScreenTestConfigurator(DomainFacade domainFacade)
        {
            if (domainFacade == null) domainFacade = new DomainFacade();

            return new FlowScreenTestConfigurator<WebApp.Flows.AppFlows.Usage.Steps.Usage, ResidentialPortalFlowType>(
                    domainFacade.ModelsBuilder)
                .WithStub(domainFacade.SessionProvider)
                .WithStub(domainFacade.QueryResolver)
                .WithStub(domainFacade.CommandDispatcher);
        }
        
        [Test]
        public void FlowTypeIsCorrect()
        {
            Assert.AreEqual(ResidentialPortalFlowType.Usage, NewScreenTestConfigurator().Adapter.GetFlowType());
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

        [Test, Theory]
        public async Task Can_Get_CorrectValuesForUsageScreenModel(bool mainAccountIsElectricityAccount, bool withDuelFuel)
        {
            var cfg = ConfigureDomain(mainAccountIsElectricityAccount, withDuelFuel);

            var rootDataBuilder = new RootDataBuilder(cfg);
            var rootStepData = rootDataBuilder.Create(mainAccountIsElectricityAccount);

            var mainAccountNumber = mainAccountIsElectricityAccount
                ? cfg.ElectricityAccount().Model.AccountNumber
                : cfg.GasAccount().Model.AccountNumber;

            NewScreenTestConfigurator(cfg.DomainFacade)
                .WithMockConfiguration<IUiAppSettings>(c =>
                {
                    c.Setup(s => s.ElectricIrelandBaseUrl).Returns("https://www.electricireland.ie");
                    c.Setup(s => s.ShopBaseUrl).Returns("https://shop.electricireland.ie");
                    c.Setup(s => s.StoreBaseUrl).Returns("https://store.electricireland.ie");
                })
                .NewTestCreateStepDataRunner()
                .WithExistingStepData(ScreenName.PreStart, rootStepData)
                .WhenCreated()
                .ThenTheStepDataIs<WebApp.Flows.AppFlows.Usage.Steps.Usage.ScreenModel>(actual =>
                {
                    Assert.AreEqual(mainAccountNumber, actual.AccountNumber);
                });

        }

    }
}
