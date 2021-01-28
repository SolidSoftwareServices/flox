using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Sut;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.ForgotPassword.Pages
{
	class RequestForgotPasswordLinkSentPage : SutPage<ResidentialPortalApp>
	{
		public RequestForgotPasswordLinkSentPage(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			return Page != null;
		}

		public IHtmlElement Page => Document.QuerySelector("[data-page='reset-password-confirmation-email-page']") as IHtmlElement;
		public IHtmlHeadingElement Heading => Page.QuerySelector("[data-testid='reset-password-confirmation-email-heading']") as IHtmlHeadingElement;
		public IHtmlElement Message => Page.QuerySelector("[data-testid='reset-password-confirmation-email-message']") as IHtmlElement;

		public IHtmlAnchorElement ResubmitEmailLink =>
			Document.QuerySelector("a#resubmitEmail") as IHtmlAnchorElement;

		public async Task<ResidentialPortalApp> ClickOnResubmitEmailLink()
		{
			return await App.ClickOnElement(ResubmitEmailLink) as ResidentialPortalApp;
		}
	}
}