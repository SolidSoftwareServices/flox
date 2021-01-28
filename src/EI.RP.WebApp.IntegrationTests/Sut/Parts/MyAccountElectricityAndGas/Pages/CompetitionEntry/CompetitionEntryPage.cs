using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.CompetitionEntry
{
	internal class CompetitionEntryPage : MyAccountElectricityAndGasPage
	{
		public CompetitionEntryPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement PageContainer => (IHtmlElement) Document.QuerySelector("[data-page='competition-entry']");

		public IHtmlElement AlreadyParticipatedError =>
			(IHtmlElement) PageContainer.QuerySelector("[data-testid='error-already-participated']");

		public IHtmlInputElement CompetitionName =>
			(IHtmlInputElement) PageContainer.QuerySelector("[data-testid='competition-entry-name']");

		public IHtmlElement CompetitionEntryImage =>
			(IHtmlElement) PageContainer.QuerySelector("[data-testid='competition-entry-image']");

		public IHtmlElement CompetitionHeading =>
			(IHtmlElement) PageContainer.QuerySelector("[data-testid='competition-heading']");

		public IHtmlElement CompetitionDescription =>
			(IHtmlElement) PageContainer.QuerySelector("[data-testid='competition-description']");

		public IHtmlElement CompetitionDescription1 =>
			(IHtmlElement) PageContainer.QuerySelector("[data-testid='competition-description1']");

		public IHtmlElement CompetitionDescription2 =>
			(IHtmlElement) PageContainer.QuerySelector("[data-testid='competition-description2']");

		public IHtmlElement CompetitionQuestion =>
			(IHtmlElement) PageContainer.QuerySelector("[data-testid='competition-question']");

		public IHtmlInputElement Answer1CheckBox =>
			(IHtmlInputElement) PageContainer.QuerySelector("[data-testid='Answer1-CheckBox']");

		public IHtmlElement CompetitionAnswer1 =>
			(IHtmlElement) PageContainer.QuerySelector("[data-testid='competition-Answer1']");

		public IHtmlInputElement Answer2CheckBox =>
			(IHtmlInputElement) PageContainer.QuerySelector("[data-testid='Answer2-CheckBox']");

		public IHtmlElement CompetitionAnswer2 =>
			(IHtmlElement) PageContainer.QuerySelector("[data-testid='competition-Answer2']");

		public IHtmlInputElement Answer3CheckBox =>
			(IHtmlInputElement) PageContainer.QuerySelector("[data-testid='Answer3-CheckBox']");

		public IHtmlElement CompetitionAnswer3 =>
			(IHtmlElement) PageContainer.QuerySelector("[data-testid='competition-Answer3']");

		public IHtmlElement CompetitionAnswerValidationError =>
			(IHtmlElement) PageContainer.QuerySelector("[data-valmsg-for='Answer']");

		public IHtmlInputElement CompetitionContactConsentCheckbox =>
			(IHtmlInputElement) PageContainer.QuerySelector("[data-testid='competition-contact-consent-checkbox']");

		public IHtmlElement CompetitionContactConsent =>
			(IHtmlElement) PageContainer.QuerySelector("[data-testid='competition-contact-consent']");

		public IHtmlElement CompetitionContactConsentValidationError =>
			(IHtmlElement) PageContainer.QuerySelector("[data-valmsg-for='Consent']");

		public IHtmlElement SubmitCompetitionEntryButton =>
			(IHtmlElement) PageContainer.QuerySelector("[data-testid='submit-competition-entry-button']");

		public IHtmlElement CompetitionEntrySuccessMessage =>
			(IHtmlElement) PageContainer.QuerySelector("[data-testid='competition-entry-success-message']");

		public IHtmlAnchorElement CompetitionEntrySuccessBackToAccounts =>
			(IHtmlAnchorElement) PageContainer.QuerySelector(
				"[data-testid='competition-entry-success-back-to-accounts']");

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
			var isInPage = base.IsInPage() && PageContainer != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Competition"));
			}

			return isInPage;
		}
	}
}