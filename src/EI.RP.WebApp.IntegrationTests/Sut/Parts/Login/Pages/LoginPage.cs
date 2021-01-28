using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Sut;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.Login.Pages
{
	class LoginPage: SutPage<ResidentialPortalApp>
	{
		public LoginPage(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			return Page != null;
		}

		public IHtmlElement Page => Document.QuerySelector("[data-page='login']") as IHtmlElement;

		public IHtmlInputElement UserNameElement => Document.QuerySelector("#txtUserName") as IHtmlInputElement;
		
		public IHtmlInputElement EmailElement => Document.QuerySelector("#txtEmail") as IHtmlInputElement;
		
		public IHtmlInputElement PasswordElement => Document.QuerySelector("#txtPassword") as IHtmlInputElement;

		public IHtmlButtonElement LoginButton => (IHtmlButtonElement)Document.QuerySelector("#btnLogin");

        public IHtmlHeadingElement LoginPageHeader =>
            (IHtmlHeadingElement) Document.QuerySelector("[data-testid='title']");

		public async Task<ResidentialPortalApp> ClickOnLoginButton()
		{
			return (ResidentialPortalApp)await App.ClickOnElement(LoginButton);

		}

		private IHtmlAnchorElement CreateAccountLink => (IHtmlAnchorElement)Document.QuerySelector("#createAccountLink");

		public async Task<ResidentialPortalApp> ClickOnCreateAccountLink()
		{
			return (ResidentialPortalApp)await App.ClickOnElement(CreateAccountLink);
		}

        private IHtmlAnchorElement ForgotPasswordLink => Document.QuerySelector("#forgotPasswordLink") as IHtmlAnchorElement;

        public async Task<ResidentialPortalApp> ClickOnForgotPasswordLink()
        {
            return (await App.ClickOnElement(ForgotPasswordLink)) as ResidentialPortalApp;
        }
	}
}
