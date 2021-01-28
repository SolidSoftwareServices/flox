using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Sut;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.CreateAccount.Pages
{
	class RegistrationLinkExpiredPage : SutPage<ResidentialPortalApp>
	{
		protected override bool IsInPage()
		{
			return Page != null;
		}

		private IHtmlElement Page =>
			Document.QuerySelector("[data-page='create-account-link-expired']") as IHtmlElement;

		public RegistrationLinkExpiredPage(ResidentialPortalApp app) : base(app)
		{
		}
	}
}
