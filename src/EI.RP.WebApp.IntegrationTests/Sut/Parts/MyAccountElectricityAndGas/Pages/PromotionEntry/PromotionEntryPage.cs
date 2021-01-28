using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.PromotionEntry
{
	internal class PromotionEntryPage : MyAccountElectricityAndGasPage
	{
		public PromotionEntryPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement PromotionPageContainer =>
			(IHtmlElement) Document.QuerySelector("[data-page='promotion-entry']");

		public IHtmlImageElement PromotionHeaderImage =>
			PromotionPageContainer.QuerySelector("[data-testid='promotion-header']") as IHtmlImageElement;

		public IHtmlElement PromotionHeading =>
			PromotionPageContainer.QuerySelector("[data-testid='promotion-heading']") as IHtmlElement;

		public IHtmlParagraphElement PromotionDescription1 =>
			PromotionPageContainer.QuerySelector("[data-testid='promotion-description1']") as IHtmlParagraphElement;

		public IHtmlParagraphElement PromotionDescription2 =>
			PromotionPageContainer.QuerySelector("[data-testid='promotion-description2']") as IHtmlParagraphElement;

		public IHtmlParagraphElement PromotionDescription3 =>
			PromotionPageContainer.QuerySelector("[data-testid='promotion-description3']") as IHtmlParagraphElement;

		public IHtmlParagraphElement PromotionDescription4 =>
			PromotionPageContainer.QuerySelector("[data-testid='promotion-description4']") as IHtmlParagraphElement;

		public IHtmlAnchorElement PromotionLink =>
			PromotionPageContainer.QuerySelector("[data-testid='promotion-link']") as IHtmlAnchorElement;

		public IHtmlAnchorElement PromotionTermsConditionLink =>
			PromotionPageContainer.QuerySelector("[data-testid='promotion-terms-and-condition']") as IHtmlAnchorElement;

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
			var isInPage = base.IsInPage() && PromotionPageContainer != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Promotion"));
			}

			return isInPage;
		}
	}
}