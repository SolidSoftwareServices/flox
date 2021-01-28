using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.WebApp.Flows.AppFlows.ProductAndServices.FlowDefinitions;
using EI.RP.WebApp.Infrastructure.Settings;
using System.Threading.Tasks;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.WebApp.Flows.AppFlows.ProductAndServices.Steps
{
    public class ProductAndServices : ProductAndServicesScreen
    {
	    private readonly IUiAppSettings _uiAppSettings;
        public override ScreenName ScreenStep => ProductAndServicesStep.ProductAndServices;

        public ProductAndServices(IUiAppSettings uiAppSettings)
        {
            _uiAppSettings = uiAppSettings;
        }

        protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
        {
            var data = new ScreenModel
            {
	            NestLearningThermostat = $"{_uiAppSettings.ShopBaseUrl}/products/install-detail/nest-thermostat",
                NestLearningThermostatFindOutMoreURL =$"{_uiAppSettings.ShopBaseUrl}/products/install-detail/nest-thermostat",
                ClimoteURL = $"{_uiAppSettings.ShopBaseUrl}/products/install-detail/climote",
                ClimoteFindOutMoreURL = $"{_uiAppSettings.ShopBaseUrl}/products/install-detail/climote",
                HoneywellEvoHomeURL = $"{_uiAppSettings.ShopBaseUrl}/products/install-detail/honeywell",
                HoneywellEvoHomeFindOutMoreURL = $"{_uiAppSettings.ShopBaseUrl}/products/install-detail/honeywell",
                GasBoilerServiceURL = $"{_uiAppSettings.ShopBaseUrl}/heating-service-repair/detail/gas-boiler-service",
                GasBoilerFindOutURL = $"{_uiAppSettings.ShopBaseUrl}/heating-service-repair/detail/gas-boiler-service",
                GasBoilerRepairURL = $"{_uiAppSettings.ShopBaseUrl}/heating-service-repair/detail/gas-boiler-repair",
                GasBoilerRepairFindOutURL = $"{_uiAppSettings.ShopBaseUrl}/heating-service-repair",
                GasBoilerReplacementURL = $"{_uiAppSettings.ShopBaseUrl}/heating-service-repair/detail/gas-boiler-replacement",
                GasBoilerReplacementFindOutURL = $"{_uiAppSettings.ShopBaseUrl}/heating-service-repair",
                NestGasBoilerServiceURL = $"{_uiAppSettings.ShopBaseUrl}/bundles/nest-gas-boiler-service",
                NestGasBoilerServiceFindOutURL = $"{_uiAppSettings.ShopBaseUrl}/bundles",
                ClimotGasBoilerServiceURL = $"{_uiAppSettings.ShopBaseUrl}/bundles/climote-gas-boiler-service",
                ClimoteGasBoilerServiceFindOutURL = $"{_uiAppSettings.ShopBaseUrl}/bundles",
                SolarPVURL = $"{_uiAppSettings.ShopBaseUrl}/solar-pv/solar-pv-calculator",
                SolarPVFindOutURL = $"{_uiAppSettings.ShopBaseUrl}/solar-calculator",
                EvChargersURL = $"{_uiAppSettings.ShopBaseUrl}/products/install-detail/electric-vehicle-home-charger",
                EvChargersFindOutURL = $"{_uiAppSettings.ShopBaseUrl}/products/install-detail/electric-vehicle-home-charger",
                SolarBatteryStorageURL = $"{_uiAppSettings.ShopBaseUrl}/products/install-detail/solar-battery-storage",
                SolarBatteryStorageFindOutURL = $"{_uiAppSettings.ShopBaseUrl}/products/install-detail/solar-battery-storage",
                NestCamIndoorURL = $"{_uiAppSettings.StoreBaseUrl}/home/20-nest-cam-indoor",
                NestCamIndoorFindOutURL = $"{_uiAppSettings.StoreBaseUrl}/home/20-nest-cam-indoor",
                NestProtectURL = $"{_uiAppSettings.StoreBaseUrl}/home/16-Nest-Protect",
                NestProtectFindOutURL = $"{_uiAppSettings.StoreBaseUrl}/home/16-Nest-Protect",
                GoogleHomeMiniURL = $"{_uiAppSettings.StoreBaseUrl}/home/17-google-home",
                GoogleHomeMiniFindOutURL = $"{_uiAppSettings.StoreBaseUrl}/home/17-google-home",
                SmarterPayAsYouGoURL = $"{_uiAppSettings.ElectricIrelandBaseUrl}/residential/products/electricity-and-gas/smarter-pay-as-you-go",
                SmarterPayAsYouGoFindOutMoreURL = $"{_uiAppSettings.ElectricIrelandBaseUrl}/residential/products/electricity-and-gas/smarter-pay-as-you-go",
                AddGasURL = $"{_uiAppSettings.ElectricIrelandBaseUrl}/switch/Customise/CustomiseLanding?pricePlanType=G",
                AddGasFindOutURL = $"{_uiAppSettings.ElectricIrelandBaseUrl}/switch/Customise/CustomiseLanding?pricePlanType=G",
                AllElectricHomeURL = $"{_uiAppSettings.ElectricIrelandBaseUrl}/residential/products/electricity-and-gas/all-electric-home",
                AllElectricHomeFindOutURL = $"{_uiAppSettings.ElectricIrelandBaseUrl}/residential/products/electricity-and-gas/all-electric-home"
            };

            SetTitle(Title, data);

            return data;
        }

        public class ScreenModel : UiFlowScreenModel
        {
            public override bool IsValidFor(ScreenName screenStep)
            {
                return screenStep == ProductAndServicesStep.ProductAndServices;
            }
            public string NestLearningThermostat { get; set; }
            public string NestLearningThermostatFindOutMoreURL { get; set; }
            public string ClimoteURL { get; set; }
            public string ClimoteFindOutMoreURL { get; set; }
            public string HoneywellEvoHomeURL { get; set; }
            public string HoneywellEvoHomeFindOutMoreURL { get; set; }
            public string GasBoilerServiceURL { get; set; }
            public string GasBoilerFindOutURL { get; set; }
            public string GasBoilerRepairURL { get; set; }
            public string GasBoilerRepairFindOutURL { get; set; }
            public string GasBoilerReplacementURL { get; set; }
            public string GasBoilerReplacementFindOutURL { get; set; }
            public string NestGasBoilerServiceURL { get; set; }
            public string NestGasBoilerServiceFindOutURL { get; set; }
            public string ClimotGasBoilerServiceURL { get; set; }
            public string ClimoteGasBoilerServiceFindOutURL { get; set; }
            public string SolarPVURL { get; set; }
            public string SolarPVFindOutURL { get; set; }
            public string EvChargersURL { get; set; }
            public string EvChargersFindOutURL { get; set; }
            public string SolarBatteryStorageURL { get; set; }
            public string SolarBatteryStorageFindOutURL { get; set; }
            public string NestCamIndoorURL { get; set; }
            public string NestCamIndoorFindOutURL { get; set; }
            public string NestProtectURL { get; set; }
            public string NestProtectFindOutURL { get; set; }
            public string GoogleHomeMiniURL { get; set; }
            public string GoogleHomeMiniFindOutURL { get; set; }
            public string SmarterPayAsYouGoURL { get; set; }
            public string SmarterPayAsYouGoFindOutMoreURL { get; set; }
            public string AddGasURL { get; set; }
            public string AddGasFindOutURL { get; set; }
            public string AllElectricHomeURL { get; set; }
            public string AllElectricHomeFindOutURL { get; set; }
        }
    }
}
