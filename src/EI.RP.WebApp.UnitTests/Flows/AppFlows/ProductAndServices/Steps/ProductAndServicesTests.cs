using Ei.Rp.DomainModels.MappingValues;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Accounts;
using EI.RP.DomainModels.SpecimenBuilders.RichBuilders.Users;
using EI.RP.UI.TestServices.Flows.FlowScreenUnitTest;
using EI.RP.WebApp.Flows.AppFlows;
using EI.RP.WebApp.Infrastructure.Settings;
using NUnit.Framework;
using System.Threading.Tasks;

namespace EI.RP.WebApp.UnitTests.Flows.AppFlows.ProductAndServices.Steps
{
    [TestFixture]
    public class ProductAndServicesTests
    {
        private FlowScreenTestConfigurator<WebApp.Flows.AppFlows.ProductAndServices.Steps.ProductAndServices, ResidentialPortalFlowType> NewScreenTestConfigurator(
            bool anonymousUser = false)
        {
            return NewScreenTestConfigurator(anonymousUser
                ? new DomainFacade()
                : new AppUserConfigurator().Execute().SetValidSession().DomainFacade);
        }

        private FlowScreenTestConfigurator<WebApp.Flows.AppFlows.ProductAndServices.Steps.ProductAndServices, ResidentialPortalFlowType> NewScreenTestConfigurator(DomainFacade domainFacade)
        {
            if (domainFacade == null) domainFacade = new DomainFacade();

            return new FlowScreenTestConfigurator<WebApp.Flows.AppFlows.ProductAndServices.Steps.ProductAndServices, ResidentialPortalFlowType>(
                    domainFacade.ModelsBuilder)
                .WithStub(domainFacade.SessionProvider)
                .WithStub(domainFacade.QueryResolver)
                .WithStub(domainFacade.CommandDispatcher);
        }
        
        [Test]
        public void FlowTypeIsCorrect()
        {
            Assert.AreEqual(ResidentialPortalFlowType.ProductAndServices, NewScreenTestConfigurator().Adapter.GetFlowType());
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
        public async Task Can_Get_CorrectLinks(bool mainAccountIsElectricityAccount, bool withDuelFuel)
        {
            var cfg = ConfigureDomain(mainAccountIsElectricityAccount, withDuelFuel);

            NewScreenTestConfigurator(cfg.DomainFacade)
                .WithMockConfiguration<IUiAppSettings>(c =>
                {
                    c.Setup(s => s.ElectricIrelandBaseUrl).Returns("https://www.electricireland.ie");
                    c.Setup(s => s.ShopBaseUrl).Returns("https://shop.electricireland.ie");
                    c.Setup(s => s.StoreBaseUrl).Returns("https://store.electricireland.ie");
                })
                .NewTestCreateStepDataRunner()
                .WhenCreated()
                .ThenTheStepDataIs<WebApp.Flows.AppFlows.ProductAndServices.Steps.ProductAndServices.ScreenModel>(actual =>
                {
                    Assert.AreEqual("https://shop.electricireland.ie/products/install-detail/nest-thermostat", actual.NestLearningThermostat);
                    Assert.AreEqual("https://shop.electricireland.ie/products/install-detail/nest-thermostat", actual.NestLearningThermostatFindOutMoreURL);
                    Assert.AreEqual("https://shop.electricireland.ie/products/install-detail/climote", actual.ClimoteURL);
                    Assert.AreEqual("https://shop.electricireland.ie/products/install-detail/climote", actual.ClimoteFindOutMoreURL);
                    Assert.AreEqual("https://shop.electricireland.ie/products/install-detail/honeywell", actual.HoneywellEvoHomeURL);
                    Assert.AreEqual("https://shop.electricireland.ie/products/install-detail/honeywell", actual.HoneywellEvoHomeFindOutMoreURL);
                    Assert.AreEqual("https://shop.electricireland.ie/heating-service-repair/detail/gas-boiler-service", actual.GasBoilerServiceURL);
                    Assert.AreEqual("https://shop.electricireland.ie/heating-service-repair/detail/gas-boiler-service", actual.GasBoilerFindOutURL);
                    Assert.AreEqual("https://shop.electricireland.ie/heating-service-repair/detail/gas-boiler-repair", actual.GasBoilerRepairURL);
                    Assert.AreEqual("https://shop.electricireland.ie/heating-service-repair", actual.GasBoilerRepairFindOutURL);
                    Assert.AreEqual("https://shop.electricireland.ie/heating-service-repair/detail/gas-boiler-replacement", actual.GasBoilerReplacementURL);
                    Assert.AreEqual("https://shop.electricireland.ie/heating-service-repair", actual.GasBoilerReplacementFindOutURL);
                    Assert.AreEqual("https://shop.electricireland.ie/bundles/nest-gas-boiler-service", actual.NestGasBoilerServiceURL);
                    Assert.AreEqual("https://shop.electricireland.ie/bundles", actual.NestGasBoilerServiceFindOutURL);
                    Assert.AreEqual("https://shop.electricireland.ie/bundles/climote-gas-boiler-service", actual.ClimotGasBoilerServiceURL);
                    Assert.AreEqual("https://shop.electricireland.ie/bundles", actual.ClimoteGasBoilerServiceFindOutURL);
                    Assert.AreEqual("https://shop.electricireland.ie/solar-pv/solar-pv-calculator", actual.SolarPVURL);
                    Assert.AreEqual("https://shop.electricireland.ie/solar-calculator", actual.SolarPVFindOutURL);
                    Assert.AreEqual("https://shop.electricireland.ie/products/install-detail/electric-vehicle-home-charger", actual.EvChargersURL);
                    Assert.AreEqual("https://shop.electricireland.ie/products/install-detail/electric-vehicle-home-charger", actual.EvChargersFindOutURL);
                    Assert.AreEqual("https://shop.electricireland.ie/products/install-detail/solar-battery-storage", actual.SolarBatteryStorageURL);
                    Assert.AreEqual("https://shop.electricireland.ie/products/install-detail/solar-battery-storage", actual.SolarBatteryStorageFindOutURL);
                    Assert.AreEqual("https://store.electricireland.ie/home/20-nest-cam-indoor", actual.NestCamIndoorURL);
                    Assert.AreEqual("https://store.electricireland.ie/home/20-nest-cam-indoor", actual.NestCamIndoorFindOutURL);
                    Assert.AreEqual("https://store.electricireland.ie/home/16-Nest-Protect", actual.NestProtectURL);
                    Assert.AreEqual("https://store.electricireland.ie/home/16-Nest-Protect", actual.NestProtectFindOutURL);
                    Assert.AreEqual("https://store.electricireland.ie/home/17-google-home", actual.GoogleHomeMiniURL);
                    Assert.AreEqual("https://store.electricireland.ie/home/17-google-home", actual.GoogleHomeMiniFindOutURL);
                    Assert.AreEqual("https://www.electricireland.ie/residential/products/electricity-and-gas/smarter-pay-as-you-go", actual.SmarterPayAsYouGoURL);
                    Assert.AreEqual("https://www.electricireland.ie/residential/products/electricity-and-gas/smarter-pay-as-you-go", actual.SmarterPayAsYouGoFindOutMoreURL);
                    Assert.AreEqual("https://www.electricireland.ie/switch/Customise/CustomiseLanding?pricePlanType=G", actual.AddGasURL);
                    Assert.AreEqual("https://www.electricireland.ie/switch/Customise/CustomiseLanding?pricePlanType=G", actual.AddGasFindOutURL);
                    Assert.AreEqual("https://www.electricireland.ie/residential/products/electricity-and-gas/all-electric-home", actual.AllElectricHomeURL);
                    Assert.AreEqual("https://www.electricireland.ie/residential/products/electricity-and-gas/all-electric-home", actual.AllElectricHomeFindOutURL);
                });

        }

    }
}
