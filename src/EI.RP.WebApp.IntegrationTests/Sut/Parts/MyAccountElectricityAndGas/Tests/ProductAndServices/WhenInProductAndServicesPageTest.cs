using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.ProductAndServices;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.CommonHeaders;
using NUnit.Framework;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Tests.ProductAndServices
{
    [TestFixture]
    class WhenInProductAndServicesPageTest : MyAccountCommonTests<ProductAndServicesPage>
    {
        protected override async Task TestScenarioArrangement()
        {
            UserConfig = App.ConfigureUser("a@A.com", "test");
            UserConfig.AddElectricityAccount().WithInvoices(3);
            UserConfig.Execute();

            var app = await ((ResidentialPortalApp)await App.WithValidSessionFor(UserConfig.UserName, UserConfig.Role)).ToFirstAccount();
            app = await app.CurrentPageAs<MyAccountElectricityAndGasPage>().ToProductAndServices();
            Sut = app.CurrentPageAs<ProductAndServicesPage>();
        }

        [Test]
        public async Task CanSeeComponentItems()
        {
            Assert.NotNull(Sut.ProductAndServicesContainer);

            Assert.NotNull(Sut.SmartThermostats);
            Assert.IsTrue(Sut.SmartThermostatsText?.TextContent?.Equals("Smart Thermostats"));

            Assert.NotNull(Sut.NestLearningThermostatCard);
            Assert.IsTrue(Sut.NestLearningThermostatCardHeading?.TextContent?.Equals("Nest Learning Thermostat"));
            Assert.IsTrue(Sut.NestLearningThermostatCardLink?.Href?.Equals("https://shop.electricireland.ie/products/install-detail/nest-thermostat"));
            Assert.IsTrue(Sut.NestLearningThermostatCardMoreLink?.Href?.Equals("https://shop.electricireland.ie/products/install-detail/nest-thermostat"));

            Assert.NotNull(Sut.ClimoteCard);
            Assert.IsTrue(Sut.ClimoteCardHeading?.TextContent?.Equals("Climote"));
            Assert.IsTrue(Sut.ClimoteCardLink?.Href?.Equals("https://shop.electricireland.ie/products/install-detail/climote"));
            Assert.IsTrue(Sut.ClimoteCardMoreLink?.Href?.Equals("https://shop.electricireland.ie/products/install-detail/climote"));

            Assert.NotNull(Sut.HoneywellEvohomeCard);
            Assert.IsTrue(Sut.HoneywellEvohomeCardHeading?.TextContent?.Equals("Honeywell Evohome"));
            Assert.IsTrue(Sut.HoneywellEvohomeCardLink?.Href?.Equals("https://shop.electricireland.ie/products/install-detail/honeywell"));
            Assert.IsTrue(Sut.HoneywellEvohomeCardMoreLink?.Href?.Equals("https://shop.electricireland.ie/products/install-detail/honeywell"));

            Assert.NotNull(Sut.GasBoilerServices);
            Assert.IsTrue(Sut.GasBoilerServicesText?.TextContent?.Equals("Gas Boiler Services"));

            Assert.NotNull(Sut.GasBoilerServiceCard);
            Assert.IsTrue(Sut.GasBoilerServiceCardHeading?.TextContent?.Equals("Gas Boiler Service"));
            Assert.IsTrue(Sut.GasBoilerServiceCardLink?.Href?.Equals("https://shop.electricireland.ie/heating-service-repair/detail/gas-boiler-service"));
            Assert.IsTrue(Sut.GasBoilerServiceCardMoreLink?.Href?.Equals("https://shop.electricireland.ie/heating-service-repair/detail/gas-boiler-service"));

            Assert.NotNull(Sut.GasBoilerRepairCard);
            Assert.IsTrue(Sut.GasBoilerRepairCardHeading?.TextContent?.Equals("Gas Boiler Repair"));
            Assert.IsTrue(Sut.GasBoilerRepairCardLink?.Href?.Equals("https://shop.electricireland.ie/heating-service-repair/detail/gas-boiler-repair"));
            Assert.IsTrue(Sut.GasBoilerRepairCardMoreLink?.Href?.Equals("https://shop.electricireland.ie/heating-service-repair"));

            Assert.NotNull(Sut.GasBoilerReplacementCard);
            Assert.IsTrue(Sut.GasBoilerReplacementCardHeading?.TextContent?.Equals("Gas Boiler Replacement"));
            Assert.IsTrue(Sut.GasBoilerReplacementCardLink?.Href?.Equals("https://shop.electricireland.ie/heating-service-repair/detail/gas-boiler-replacement"));
            Assert.IsTrue(Sut.GasBoilerReplacementCardMoreLink?.Href?.Equals("https://shop.electricireland.ie/heating-service-repair"));

            Assert.NotNull(Sut.SmartThermostatAndGasBoilerServices);
            Assert.IsTrue(Sut.SmartThermostatAndGasBoilerServicesText?.TextContent?.Equals("Smart Thermostat and Gas Boiler Service Bundles"));

            Assert.NotNull(Sut.NestGasBoilerServiceCard);
            Assert.IsTrue(Sut.NestGasBoilerServiceCardHeading?.TextContent?.Equals("Nest & Gas Boiler Service"));
            Assert.IsTrue(Sut.NestGasBoilerServiceCardLink?.Href?.Equals("https://shop.electricireland.ie/bundles/nest-gas-boiler-service"));
            Assert.IsTrue(Sut.NestGasBoilerServiceCardMoreLink?.Href?.Equals("https://shop.electricireland.ie/bundles"));

            Assert.NotNull(Sut.ClimoteGasBoilerServiceCard);
            Assert.IsTrue(Sut.ClimoteGasBoilerServiceCardHeading?.TextContent?.Equals("Climote & Gas Boiler Services"));
            Assert.IsTrue(Sut.ClimoteGasBoilerServiceCardLink?.Href?.Equals("https://shop.electricireland.ie/bundles/climote-gas-boiler-service"));
            Assert.IsTrue(Sut.ClimoteGasBoilerServiceCardMoreLink?.Href?.Equals("https://shop.electricireland.ie/bundles"));

            Assert.NotNull(Sut.HomeEnergyProducts);
            Assert.IsTrue(Sut.HomeEnergyProductsText?.TextContent?.Equals("Home Energy Products"));

            Assert.NotNull(Sut.SolarPvCard);
            Assert.IsTrue(Sut.SolarPvCardHeading?.TextContent?.Equals("Solar PV"));
            Assert.IsTrue(Sut.SolarPvCardLink?.Href?.Equals("https://shop.electricireland.ie/solar-pv/solar-pv-calculator"));
            Assert.IsTrue(Sut.SolarPvCardMoreLink?.Href?.Equals("https://shop.electricireland.ie/solar-calculator"));

            Assert.NotNull(Sut.EvChargersCard);
            Assert.IsTrue(Sut.EvChargersCardHeading?.TextContent?.Equals("EV Chargers"));
            Assert.IsTrue(Sut.EvChargersCardLink?.Href?.Equals("https://shop.electricireland.ie/products/install-detail/electric-vehicle-home-charger"));
            Assert.IsTrue(Sut.EvChargersCardMoreLink?.Href?.Equals("https://shop.electricireland.ie/products/install-detail/electric-vehicle-home-charger"));

            Assert.NotNull(Sut.SolarBatteryStorageCard);
            Assert.IsTrue(Sut.SolarBatteryStorageCardHeading?.TextContent?.Equals("Solar Battery Storage"));
            Assert.IsTrue(Sut.SolarBatteryStorageCardLink?.Href?.Equals("https://shop.electricireland.ie/products/install-detail/solar-battery-storage"));
            Assert.IsTrue(Sut.SolarBatteryStorageCardMoreLink?.Href?.Equals("https://shop.electricireland.ie/products/install-detail/solar-battery-storage"));

            Assert.NotNull(Sut.SmartHomeProducts);
            Assert.IsTrue(Sut.SmartHomeProductsText?.TextContent?.Equals("Smart Home Products"));

            Assert.NotNull(Sut.NestCamIndoorCard);
            Assert.IsTrue(Sut.NestCamIndoorCardHeading?.TextContent?.Equals("Nest Cam Indoor"));
            Assert.IsTrue(Sut.NestCamIndoorCardLink?.Href?.Equals("https://store.electricireland.ie/home/20-nest-cam-indoor"));
            Assert.IsTrue(Sut.NestCamIndoorCardMoreLink?.Href?.Equals("https://store.electricireland.ie/home/20-nest-cam-indoor"));

            Assert.NotNull(Sut.NestProtectCard);
            Assert.IsTrue(Sut.NestProtectCardHeading?.TextContent?.Equals("Nest Protect"));
            Assert.IsTrue(Sut.NestProtectCardLink?.Href?.Equals("https://store.electricireland.ie/home/16-Nest-Protect"));
            Assert.IsTrue(Sut.NestProtectCardMoreLink?.Href?.Equals("https://store.electricireland.ie/home/16-Nest-Protect"));

            Assert.NotNull(Sut.GoogleHomeMiniCard);
            Assert.IsTrue(Sut.GoogleHomeMiniCardHeading?.TextContent?.Equals("Google Home Mini"));
            Assert.IsTrue(Sut.GoogleHomeMiniCardLink?.Href?.Equals("https://store.electricireland.ie/home/17-google-home"));
            Assert.IsTrue(Sut.GoogleHomeMiniCardMoreLink?.Href?.Equals("https://store.electricireland.ie/home/17-google-home"));

            Assert.NotNull(Sut.InterestedProducts);
            Assert.IsTrue(Sut.InterestedProductsText?.TextContent?.Equals("You may also be interested in..."));

            Assert.NotNull(Sut.SmarterPayAsYouGoCard);
            Assert.IsTrue(Sut.SmarterPayAsYouGoCardHeading?.TextContent?.Equals("Smarter Pay As You Go"));
            Assert.IsTrue(Sut.SmarterPayAsYouGoCardLink?.Href?.Equals("https://www.electricireland.ie/residential/products/electricity-and-gas/smarter-pay-as-you-go"));
            Assert.IsTrue(Sut.SmarterPayAsYouGoCardMoreLink?.Href?.Equals("https://www.electricireland.ie/residential/products/electricity-and-gas/smarter-pay-as-you-go"));

            Assert.NotNull(Sut.AddGasCard);
            Assert.IsTrue(Sut.AddGasCardHeading?.TextContent?.Equals("Add Gas"));
            Assert.IsTrue(Sut.AddGasCardLink?.Href?.Equals("https://www.electricireland.ie/switch/Customise/CustomiseLanding?pricePlanType=G"));
            Assert.IsTrue(Sut.AddGasCardMoreLink?.Href?.Equals("https://www.electricireland.ie/switch/Customise/CustomiseLanding?pricePlanType=G"));

            Assert.NotNull(Sut.HeatPumpPricePlanCard);
            Assert.IsTrue(Sut.HeatPumpPricePlanCardHeading?.TextContent?.Equals("Heat Pump Price Plan"));
            Assert.IsTrue(Sut.HeatPumpPricePlanCardLink?.Href?.Equals("https://www.electricireland.ie/residential/products/electricity-and-gas/all-electric-home"));
            Assert.IsTrue(Sut.HeatPumpPricePlanCardMoreLink?.Href?.Equals("https://www.electricireland.ie/residential/products/electricity-and-gas/all-electric-home"));
        }

         [Test]
        public async Task CanSeeAllowedMenuItemsInProductsAndServicesPage()
        {
            Assert.NotNull(Sut.ChangePasswordProfileMenuItem);
            Assert.NotNull(Sut.LogoutProfileMenuItem);            
        }

        [Test]
        public async Task CannotSeeForbiddenMenuItemsInProductsAndServicesPage()
        {
            Assert.Null(Sut.MyDetailsProfileMenuItem);
            Assert.Null(Sut.MarketingProfileMenuItem);
        }
    }
}
