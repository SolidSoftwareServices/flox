using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.ProductAndServices
{
	internal class ProductAndServicesPage : MyAccountElectricityAndGasPage
	{
		public ProductAndServicesPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement ProductAndServicesPageContainer =>
			(IHtmlElement) Document.QuerySelector("[data-page='products-and-services']");

		public IHtmlElement ProductAndServicesContainer =>
			(IHtmlElement) ProductAndServicesPageContainer.QuerySelector(
				"[data-testid='products-and-services-container']");

		public IHtmlElement SmartThermostats =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='smart-thermostats']");

		public IHtmlElement SmartThermostatsText =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='smart-thermostats-heading']");

		public IHtmlElement NestLearningThermostatCard =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='nest-learning-thermostat-card']");

		public IHtmlElement NestLearningThermostatCardHeading =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='nest-learning-thermostat-card-heading']");

		public IHtmlAnchorElement NestLearningThermostatCardLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='nest-learning-thermostat-card-link']");

		public IHtmlAnchorElement NestLearningThermostatCardMoreLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='nest-learning-thermostat-card-more-link']");

		public IHtmlElement ClimoteCard =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='climote-card']");

		public IHtmlElement ClimoteCardHeading =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='climote-card-heading']");

		public IHtmlAnchorElement ClimoteCardLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector("[data-testid='climote-card-link']");

		public IHtmlAnchorElement ClimoteCardMoreLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector("[data-testid='climote-card-more-link']");

		public IHtmlElement HoneywellEvohomeCard =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='honeywell-evohome-card']");

		public IHtmlElement HoneywellEvohomeCardHeading =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='honeywell-evohome-card-heading']");

		public IHtmlAnchorElement HoneywellEvohomeCardLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='honeywell-evohome-card-link']");

		public IHtmlAnchorElement HoneywellEvohomeCardMoreLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='honeywell-evohome-card-more-link']");

		public IHtmlElement GasBoilerServices =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='gas-boiler-services']");

		public IHtmlElement GasBoilerServicesText =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='gas-boiler-services-heading']");

		public IHtmlElement GasBoilerServiceCard =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='gas-boiler-service-card']");

		public IHtmlElement GasBoilerServiceCardHeading =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='gas-boiler-service-card-heading']");

		public IHtmlAnchorElement GasBoilerServiceCardLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='gas-boiler-service-card-link']");

		public IHtmlAnchorElement GasBoilerServiceCardMoreLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='gas-boiler-service-card-more-link']");

		public IHtmlElement GasBoilerRepairCard =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='gas-boiler-repair-card']");

		public IHtmlElement GasBoilerRepairCardHeading =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='gas-boiler-repair-card-heading']");

		public IHtmlAnchorElement GasBoilerRepairCardLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='gas-boiler-repair-card-link']");

		public IHtmlAnchorElement GasBoilerRepairCardMoreLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='gas-boiler-repair-card-more-link']");

		public IHtmlElement GasBoilerReplacementCard =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='gas-boiler-replacement-card']");

		public IHtmlElement GasBoilerReplacementCardHeading =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='gas-boiler-replacement-card-heading']");

		public IHtmlAnchorElement GasBoilerReplacementCardLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='gas-boiler-replacement-card-link']");

		public IHtmlAnchorElement GasBoilerReplacementCardMoreLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='gas-boiler-replacement-card-more-link']");

		public IHtmlElement SmartThermostatAndGasBoilerServices =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='smart-thermostat-and-gas-boiler-services']");

		public IHtmlElement SmartThermostatAndGasBoilerServicesText =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='smart-thermostat-and-gas-boiler-services-heading']");

		public IHtmlElement NestGasBoilerServiceCard =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='nest-gas-boiler-service-card']");

		public IHtmlElement NestGasBoilerServiceCardHeading =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='nest-gas-boiler-service-card-heading']");

		public IHtmlAnchorElement NestGasBoilerServiceCardLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='nest-gas-boiler-service-card-link']");

		public IHtmlAnchorElement NestGasBoilerServiceCardMoreLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='nest-gas-boiler-service-card-more-link']");

		public IHtmlElement ClimoteGasBoilerServiceCard =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='climote-gas-boiler-service-card']");

		public IHtmlElement ClimoteGasBoilerServiceCardHeading =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='climote-gas-boiler-service-card-heading']");

		public IHtmlAnchorElement ClimoteGasBoilerServiceCardLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='climote-gas-boiler-service-card-link']");

		public IHtmlAnchorElement ClimoteGasBoilerServiceCardMoreLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='climote-gas-boiler-service-card-more-link']");

		public IHtmlElement HomeEnergyProducts =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='home-energy-products']");

		public IHtmlElement HomeEnergyProductsText =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='home-energy-products-heading']");

		public IHtmlElement SolarPvCard =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='solar-pv-card']");

		public IHtmlElement SolarPvCardHeading =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='solar-pv-card-heading']");

		public IHtmlAnchorElement SolarPvCardLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector("[data-testid='solar-pv-card-link']");

		public IHtmlAnchorElement SolarPvCardMoreLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector("[data-testid='solar-pv-card-more-link']");

		public IHtmlElement EvChargersCard =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='ev-chargers-card']");

		public IHtmlElement EvChargersCardHeading =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='ev-chargers-card-heading']");

		public IHtmlAnchorElement EvChargersCardLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector("[data-testid='ev-chargers-card-link']");

		public IHtmlAnchorElement EvChargersCardMoreLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='ev-chargers-card-more-link']");

		public IHtmlElement SolarBatteryStorageCard =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='solar-battery-storage-card']");

		public IHtmlElement SolarBatteryStorageCardHeading =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='solar-battery-storage-card-heading']");

		public IHtmlAnchorElement SolarBatteryStorageCardLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='solar-battery-storage-card-link']");

		public IHtmlAnchorElement SolarBatteryStorageCardMoreLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='solar-battery-storage-card-more-link']");

		public IHtmlElement SmartHomeProducts =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='smart-home-products']");

		public IHtmlElement SmartHomeProductsText =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='smart-home-products-heading']");

		public IHtmlElement NestCamIndoorCard =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='nest-cam-indoor-card']");

		public IHtmlElement NestCamIndoorCardHeading =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='nest-cam-indoor-card-heading']");

		public IHtmlAnchorElement NestCamIndoorCardLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector("[data-testid='nest-cam-indoor-card-link']");

		public IHtmlAnchorElement NestCamIndoorCardMoreLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='nest-cam-indoor-card-more-link']");

		public IHtmlElement NestProtectCard =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='nest-protect-card']");

		public IHtmlElement NestProtectCardHeading =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='nest-protect-card-heading']");

		public IHtmlAnchorElement NestProtectCardLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector("[data-testid='nest-protect-card-link']");

		public IHtmlAnchorElement NestProtectCardMoreLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='nest-protect-card-more-link']");

		public IHtmlElement GoogleHomeMiniCard =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='google-home-mini-card']");

		public IHtmlElement GoogleHomeMiniCardHeading =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='google-home-mini-card-heading']");

		public IHtmlAnchorElement GoogleHomeMiniCardLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='google-home-mini-card-link']");

		public IHtmlAnchorElement GoogleHomeMiniCardMoreLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='google-home-mini-card-more-link']");

		public IHtmlElement InterestedProducts =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='interested-products']");

		public IHtmlElement InterestedProductsText =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='interested-products-heading']");

		public IHtmlElement SmarterPayAsYouGoCard =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='smarter-pay-as-you-go-card']");

		public IHtmlElement SmarterPayAsYouGoCardHeading =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='smarter-pay-as-you-go-card-heading']");

		public IHtmlAnchorElement SmarterPayAsYouGoCardLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='smarter-pay-as-you-go-card-link']");

		public IHtmlAnchorElement SmarterPayAsYouGoCardMoreLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='smarter-pay-as-you-go-card-more-link']");

		public IHtmlElement AddGasCard =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='add-gas-card']");

		public IHtmlElement AddGasCardHeading =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='add-gas-card-heading']");

		public IHtmlAnchorElement AddGasCardLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector("[data-testid='add-gas-card-link']");

		public IHtmlAnchorElement AddGasCardMoreLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector("[data-testid='add-gas-card-more-link']");

		public IHtmlElement HeatPumpPricePlanCard =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector("[data-testid='heat-pump-price-plan-card']");

		public IHtmlElement HeatPumpPricePlanCardHeading =>
			(IHtmlElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='heat-pump-price-plan-card-heading']");

		public IHtmlAnchorElement HeatPumpPricePlanCardLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='heat-pump-price-plan-card-link']");

		public IHtmlAnchorElement HeatPumpPricePlanCardMoreLink =>
			(IHtmlAnchorElement) ProductAndServicesContainer.QuerySelector(
				"[data-testid='heat-pump-price-plan-card-more-link']");

		public IHtmlAnchorElement MyDetailsProfileMenuItem => Document
		  .QuerySelector("[data-testid='main-navigation-my-profile-link-desktop']") as IHtmlAnchorElement;

		public IHtmlAnchorElement ChangePasswordProfileMenuItem => Document
			.QuerySelector("[data-testid='main-navigation-change-password-link-desktop']") as IHtmlAnchorElement;

		public IHtmlAnchorElement MarketingProfileMenuItem => Document
			.QuerySelector("[data-testid='main-navigation-marketing-link-desktop']") as IHtmlAnchorElement;

		public IHtmlAnchorElement LogoutProfileMenuItem => Document
			.QuerySelector("[data-testid='main-navigation-log-out-link-desktop']") as IHtmlAnchorElement;

		protected override bool IsInPage()
		{
			var isInPage = Document.QuerySelector("[data-page='products-and-services']") != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Products & Services"));
			}

			return isInPage;
		}
	}
}