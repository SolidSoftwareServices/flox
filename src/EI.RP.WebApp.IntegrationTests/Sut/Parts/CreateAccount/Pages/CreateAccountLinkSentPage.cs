using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Sut;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.CreateAccount.Pages
{
	class CreateAccountLinkSentPage : SutPage<ResidentialPortalApp>
	{
		protected override bool IsInPage()
		{
            return Page != null;
		}

		public IHtmlElement Page => Document.QuerySelector("[data-page='create-account-confirmation-email-sent']") as IHtmlElement;

		public IHtmlHeadingElement Header => (IHtmlHeadingElement)Document.QuerySelector("#wrapperDiv > div > div > h1.create-account-confirmation-email-page");

		public CreateAccountLinkSentPage(ResidentialPortalApp app) : base(app)
		{
		}
	}
}