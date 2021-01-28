using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Http;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation.Components
{
	class PlanCardPageComponent
	{
		private readonly ResidentialPortalApp _app;
		private readonly IHtmlDivElement _container;

		public PlanCardPageComponent(ResidentialPortalApp app, IHtmlDivElement container)
		{
			_app = app;
			_container = container;
		}

		public string PlanName => _container.GetElementById("plan-name").Text();

		public string BadgeText =>
			_container.GetElementsByClassName("badge").SingleOrDefault()?.Text().Trim().Replace("\n", "").Replace("\t", "");

		public IHtmlButtonElement SelectPlanButton => _container.GetElementById("selectPlanBtn") as IHtmlButtonElement;
		public async Task<ResidentialPortalApp> ClickSelectPlan()
		{
			return (ResidentialPortalApp) await _app.ClickOnElement(SelectPlanButton);
		}

		public IHtmlAnchorElement FullPricingInfoLink => _container.QuerySelector("[data-testid='full-pricing-link']") as IHtmlAnchorElement;
		public IHtmlDivElement FullPricingInfoContainer => FullPricingInfoLink?.Attributes["data-target"]?.Value !=null ? _app.CurrentPage.Document.QuerySelector(FullPricingInfoLink?.Attributes["data-target"]?.Value) as IHtmlDivElement: null;

		public IHtmlTableElement ElectricityUnitPrices => FullPricingInfoContainer?.QuerySelector("[data-testid='electricity-prices']") as IHtmlTableElement;
		public IHtmlTableElement ElectricityStandingCharges => FullPricingInfoContainer?.QuerySelector("[data-testid='electricity-charges']") as IHtmlTableElement;
		public IHtmlTableElement ElectricityPublicServiceObligationLevy => FullPricingInfoContainer?.QuerySelector("[data-testid='electricity-pso']") as IHtmlTableElement;
		public IHtmlTableElement GasUnitPrices => FullPricingInfoContainer?.QuerySelector("[data-testid='gas-prices']") as IHtmlTableElement;
		public IHtmlTableElement GasStandingCharges => FullPricingInfoContainer?.QuerySelector("[data-testid='gas-charges']") as IHtmlTableElement;
		public IHtmlTableElement GasCarbonTax => FullPricingInfoContainer?.QuerySelector("[data-testid='gas-carbon-tax']") as IHtmlTableElement;

		public IHtmlElement PriceValidityMessage => FullPricingInfoContainer?.QuerySelector("[data-testid='price-validity-message']") as IHtmlElement;
		public IHtmlElement LowCostMessage => FullPricingInfoContainer?.QuerySelector("[data-testid='low-cost-message']") as IHtmlElement;
		public IHtmlElement EstimatedAnnualBillMessage => FullPricingInfoContainer?.QuerySelector("[data-testid='eab-message']") as IHtmlElement;
	}
}
