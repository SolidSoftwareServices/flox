using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Sut;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.ForgotPassword.Pages
{
    class ForgotPasswordChangePage : SutPage<ResidentialPortalApp>
    {
        public ForgotPasswordChangePage(ResidentialPortalApp app) : base(app)
        {
        }

        protected override bool IsInPage()
        {
            return Heading?.TextContent == "Create Password";
        }

        public IHtmlHeadingElement Heading => 
            Document.QuerySelector("h1.page-login__title.forgot-password-change-page") as IHtmlHeadingElement;

        public IHtmlInputElement NewPasswordInput =>
            Document.QuerySelector("input#signupPassword") as IHtmlInputElement;

        public IHtmlButtonElement ContinueButton =>
            Document.QuerySelector("form#forceChangePass button[type='submit']") as IHtmlButtonElement;

        public async Task<ResidentialPortalApp> ClickOnContinueButton()
        {
            return (await App.ClickOnElement(ContinueButton)) as ResidentialPortalApp;
        }
    }
}
