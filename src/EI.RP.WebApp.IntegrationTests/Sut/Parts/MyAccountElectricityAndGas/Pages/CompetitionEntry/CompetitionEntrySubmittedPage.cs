using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.CompetitionEntry
{
	internal class CompetitionEntrySubmittedPage : MyAccountElectricityAndGasPage
	{
		public CompetitionEntrySubmittedPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement PageContainer =>
			(IHtmlElement) Document.QuerySelector("[data-page='competition-entry-successful']");

		public IHtmlElement CompetitionEntryImage =>
			(IHtmlElement) PageContainer.QuerySelector("[data-testid='competition-entry-image']");

		public IHtmlElement CompetitionEntrySuccessMessage =>
			(IHtmlElement) PageContainer.QuerySelector("[data-testid='competition-entry-success-message']");

		public IHtmlAnchorElement CompetitionEntrySuccessBackToAccounts =>
			(IHtmlAnchorElement) PageContainer.QuerySelector(
				"[data-testid='competition-entry-success-back-to-accounts']");

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