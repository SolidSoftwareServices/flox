using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Sut;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using System.Threading.Tasks;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.ForgotPassword.Pages
{
    class RequestForgotPasswordPage : SutPage<ResidentialPortalApp>
    {
        public RequestForgotPasswordPage(ResidentialPortalApp app) : base(app)
        {
        }

        protected override bool IsInPage()
        {
            return Heading?.TextContent == "Password Reset" && ResetMyPasswordButton != null;
        }

        public IHtmlHeadingElement Heading => Document.QuerySelector("h1.page-login__title") as IHtmlHeadingElement;
        public IHtmlInputElement EmailInput => Document.QuerySelector("input#txtEmail") as IHtmlInputElement;
        public IHtmlButtonElement ResetMyPasswordButton => Document.QuerySelector("button#btnResetPassword") as IHtmlButtonElement;
        public IHtmlElement ValidationErrors => Document.QuerySelector("[data-testid='validation_errors']") as IHtmlElement;

		public async Task<ResidentialPortalApp> ClickOnResetMyPasswordButton()
        {
            return (await App.ClickOnElement(ResetMyPasswordButton)) as ResidentialPortalApp;
        }
    }
}