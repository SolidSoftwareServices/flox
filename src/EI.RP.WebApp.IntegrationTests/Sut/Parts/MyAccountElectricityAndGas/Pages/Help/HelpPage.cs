using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Help
{
	internal class HelpPage : MyAccountElectricityAndGasPage
	{
		public HelpPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement BillingContainer =>
			(IHtmlElement) Document.QuerySelector("[data-testid='billing-container']");

		public IHtmlSpanElement BillingTitle =>
			(IHtmlSpanElement) BillingContainer?.QuerySelector("[data-testid='billing-title'] > span");

		public IHtmlAnchorElement BillingLink =>
			(IHtmlAnchorElement) BillingContainer?.QuerySelector("a[data-testid='billing-link']");

		public IHtmlElement MovingHomeContainer =>
			(IHtmlElement) Document.QuerySelector("[data-testid='moving-home-container']");

		public IHtmlSpanElement MovingHomeTitle =>
			(IHtmlSpanElement) MovingHomeContainer?.QuerySelector("[data-testid='moving-home-title'] > span");

		public IHtmlAnchorElement MovingHomeLink =>
			(IHtmlAnchorElement) MovingHomeContainer?.QuerySelector("a[data-testid='moving-home-link']");

		public IHtmlElement MetersContainer =>
			(IHtmlElement) Document.QuerySelector("[data-testid='meters-container']");

		public IHtmlSpanElement MetersTitle =>
			(IHtmlSpanElement) MetersContainer?.QuerySelector("[data-testid='meters-title'] > span");

		public IHtmlAnchorElement MetersLink =>
			(IHtmlAnchorElement) MetersContainer?.QuerySelector("a[data-testid='meters-link']");

		public IHtmlElement EfficiencyContainer =>
			(IHtmlElement) Document.QuerySelector("[data-testid='efficiency-container']");

		public IHtmlSpanElement EfficiencyTitle =>
			(IHtmlSpanElement) EfficiencyContainer?.QuerySelector("[data-testid='efficiency-title'] > span");

		public IHtmlAnchorElement EfficiencyLink =>
			(IHtmlAnchorElement) EfficiencyContainer?.QuerySelector("a[data-testid='efficiency-link']");

		public IHtmlElement SpecialNeedsContainer =>
			(IHtmlElement) Document.QuerySelector("[data-testid='special-needs-container']");

		public IHtmlSpanElement SpecialNeedsTitle =>
			(IHtmlSpanElement) SpecialNeedsContainer?.QuerySelector("[data-testid='special-needs-title'] > span");

		public IHtmlAnchorElement SpecialNeedsLink =>
			(IHtmlAnchorElement) SpecialNeedsContainer?.QuerySelector("a[data-testid='special-needs-link']");

		public IHtmlElement OnlineBillingContainer =>
			(IHtmlElement) Document.QuerySelector("[data-testid='online-billing-container']");

		public IHtmlSpanElement OnlineBillingTitle =>
			(IHtmlSpanElement) OnlineBillingContainer?.QuerySelector("[data-testid='online-billing-title'] > span");

		public IHtmlAnchorElement OnlineBillingLink =>
			(IHtmlAnchorElement) OnlineBillingContainer?.QuerySelector("a[data-testid='online-billing-link']");

		public IHtmlElement MicroGenerationContainer =>
			(IHtmlElement) Document.QuerySelector("[data-testid='micro-generation-container']");

		public IHtmlSpanElement MicroGenerationTitle =>
			(IHtmlSpanElement) MicroGenerationContainer?.QuerySelector("[data-testid='micro-generation-title'] > span");

		public IHtmlAnchorElement MicroGenerationLink =>
			(IHtmlAnchorElement) MicroGenerationContainer?.QuerySelector("a[data-testid='micro-generation-link']");

		public IHtmlElement SafetyContainer =>
			(IHtmlElement) Document.QuerySelector("[data-testid='safety-container']");

		public IHtmlSpanElement SafetyTitle =>
			(IHtmlSpanElement) SafetyContainer?.QuerySelector("[data-testid='safety-title'] > span");

		public IHtmlAnchorElement SafetyLink =>
			(IHtmlAnchorElement) SafetyContainer?.QuerySelector("a[data-testid='safety-link']");

		public IHtmlElement RewardsProgrammeContainer =>
			(IHtmlElement) Document.QuerySelector("[data-testid='rewards-programme-container']");

		public IHtmlSpanElement RewardsProgrammeTitle =>
			(IHtmlSpanElement) RewardsProgrammeContainer?.QuerySelector(
				"[data-testid='rewards-programme-title'] > span");

		public IHtmlAnchorElement RewardsProgrammeLink =>
			(IHtmlAnchorElement) RewardsProgrammeContainer?.QuerySelector("a[data-testid='rewards-programme-link']");

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
			var isInPage = Document.QuerySelector("[data-page='help']") != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Help"));
			}

			return isInPage;
		}
	}
}