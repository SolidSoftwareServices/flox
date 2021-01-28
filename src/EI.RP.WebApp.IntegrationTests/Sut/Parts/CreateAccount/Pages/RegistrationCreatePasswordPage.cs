using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Sut;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.CreateAccount.Pages
{
	class RegistrationCreatePasswordPage : SutPage<ResidentialPortalApp>
	{

		public RegistrationCreatePasswordPage(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			return Page != null;
		}

		public IHtmlElement Page =>
			Document.QuerySelector("[data-page='create-account-create-password']") as IHtmlElement;

		public IHtmlHeadingElement Title => (IHtmlHeadingElement)Page.QuerySelector("#page-login__title");

		public IHtmlInputElement Password1 => (IHtmlInputElement)Page.QuerySelector("#password");
		public IHtmlElement PasswordError => (IHtmlElement)Page.QuerySelector("[data-testid='password-error']");
		public IHtmlInputElement Password2 => (IHtmlInputElement)Page.QuerySelector("#txtConfirmPassword");
		public IHtmlElement ConfirmPasswordError => (IHtmlElement)Page.QuerySelector("[data-testid='confirm-password-error']");
		public IHtmlInputElement DayOfBirth => (IHtmlInputElement)Page.QuerySelector("#txtDateofBirthDay");
		public IHtmlInputElement MonthOfBirth => (IHtmlInputElement)Page.QuerySelector("#txtDateofBirthMonth");
		public IHtmlInputElement YearOfBirth => (IHtmlInputElement)Page.QuerySelector("#txtDateofBirthYear");
		public IHtmlElement DateOfBirthError => (IHtmlElement)Page.QuerySelector("[data-testid='date-of-birth-error']");
		private IHtmlButtonElement SubmitButton => (IHtmlButtonElement)Page.QuerySelector("#btnContinue");
		public async Task<ResidentialPortalApp> ClickOnSubmitButton()
		{
			return (ResidentialPortalApp)await App.ClickOnElement(SubmitButton);
		}
	}
}