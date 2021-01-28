using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Sut;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.ForgotPassword.Pages
{
    class ForgotPasswordLinkExpiredPage : SutPage<ResidentialPortalApp>
    {
        public ForgotPasswordLinkExpiredPage(ResidentialPortalApp app) : base(app)
        {
        }

        protected override bool IsInPage()
        {
            return Heading?.TextContent == "This Link has expired";
        }

        public IHtmlHeadingElement Heading =>
            Document.QuerySelector("h1.page-login__title.reset-password-link-expired-page") as IHtmlHeadingElement;

        public IHtmlAnchorElement ResetPasswordAgainLink =>
            Document.QuerySelector("a#ResetPasswordAgain") as IHtmlAnchorElement;

        public IHtmlAnchorElement SignInLink =>
            Document.QuerySelector("a#SignUp") as IHtmlAnchorElement;

        public async Task<ResidentialPortalApp> ClickOnResetPasswordAgainLink()
        {
            return await App.ClickOnElement(ResetPasswordAgainLink) as ResidentialPortalApp;
        }

        public async Task<ResidentialPortalApp> ClickOnSignInLink()
        {
            return (await App.ClickOnElement(SignInLink)) as ResidentialPortalApp;
        }
    }
}
