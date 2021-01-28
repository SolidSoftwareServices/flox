using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Sut;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.CreateAccount.Pages
{
	class CreateAccountPage: SutPage<ResidentialPortalApp>
	{
		public CreateAccountPage(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			return Page != null;
		}

		public IHtmlElement Page => Document.QuerySelector("[data-page='create-account']") as IHtmlElement;

		public IHtmlHeadingElement SmallHeader => (IHtmlHeadingElement)Page.QuerySelector("#registerSubtitle");

		public IHtmlInputElement AccountNumber => (IHtmlInputElement)Page.QuerySelector("#txtAccountNumber");
		public IHtmlDivElement AccountNumberError => (IHtmlDivElement)Page.QuerySelector("#accountNumberErrMsg");

		public IHtmlInputElement Mprn => (IHtmlInputElement)Page.QuerySelector("#txtMPRN");
		public IHtmlSpanElement MprnError => Page.QuerySelector("[data-testid='mprn-gprn-error']") as IHtmlSpanElement;

		public IHtmlInputElement Email => (IHtmlInputElement)Page.QuerySelector("#txtEmail");
		public IHtmlDivElement EmailError => (IHtmlDivElement)Page.QuerySelector("#emailErrMsg");

		public IHtmlInputElement PhoneNumber => (IHtmlInputElement)Page.QuerySelector("#txtPhoneNumber");
		public IHtmlDivElement PhoneNumberError => (IHtmlDivElement)Page.QuerySelector("#phoneNumberErrMsg");

		public IHtmlInputElement DayOfBirth => (IHtmlInputElement)Page.QuerySelector("#txtDateofBirthDay");
		public IHtmlInputElement MonthOfBirth => (IHtmlInputElement)Page.QuerySelector("#txtDateofBirthMonth");
		public IHtmlInputElement YearOfBirth => (IHtmlInputElement)Page.QuerySelector("#txtDateofBirthYear");
		public IHtmlSpanElement DateOfBirthError => Page.QuerySelector("[data-testid='date-of-birth-error']") as IHtmlSpanElement;

		public IHtmlInputElement CheckBoxAccountOwner => (IHtmlInputElement)Page.QuerySelector("#chkOwner");
		public IHtmlInputElement CheckBoxAcceptTermsConditions => (IHtmlInputElement)Page.QuerySelector("#chkAcceptTC");

		public IHtmlButtonElement CreateAccountButton => (IHtmlButtonElement)Page.QuerySelector("#CreateAccount");

		public async Task<ResidentialPortalApp> ClickOnCreateAccount()
		{
			return (ResidentialPortalApp)await App.ClickOnElement(CreateAccountButton);

		}
	}
}
