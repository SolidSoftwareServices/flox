using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Sut;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages
{
	internal class CollectiveAccountErrorPage : SutPage<ResidentialPortalApp>
    {
	    public CollectiveAccountErrorPage(ResidentialPortalApp app) : base(app)
	    {
	    }

	    protected override bool IsInPage()
	    {
			var isInPage = Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Dashboard"));
			}

		    return isInPage;
	    }

	    public IHtmlElement Page => Document.QuerySelector("[data-page='business-online-account-error']") as IHtmlElement;
	    public IHtmlElement ErrorHeading  => Page.QuerySelector("[data-testid='business-online-account-error']") as IHtmlElement;
	    public IHtmlElement ErrorText => Page.QuerySelector("[data-testid='business-online-account-error-text']") as IHtmlElement;

		public IHtmlAnchorElement ChangePasswordMenuItemLink => (IHtmlAnchorElement)Document.QuerySelector("[data-testid='main-navigation-change-password-link-desktop']");
		public IHtmlAnchorElement LogoutMenuItemLink => (IHtmlAnchorElement)Document.QuerySelector("[data-testid='main-navigation-log-out-link-desktop']");
	}
}
