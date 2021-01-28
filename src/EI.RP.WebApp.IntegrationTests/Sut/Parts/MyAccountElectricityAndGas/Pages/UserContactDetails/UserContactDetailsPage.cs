using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.UserContactDetails
{
	internal class UserContactDetailsPage : MyAccountElectricityAndGasPage
	{
		public UserContactDetailsPage(ResidentialPortalApp app) : base(app)
		{
		}
		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("My Details"));
			}

			return isInPage;
		}

		public IHtmlElement Page => Document.QuerySelector("[data-pageid='contact-user-detail']") as IHtmlElement;
		public IHtmlHeadingElement ContactDetailsHeading => Page.QuerySelector("[data-testid='contact-details-heading']") as IHtmlHeadingElement;

		public IHtmlElement PrivacyComponent => Page.QuerySelector("[data-testid='privacy-notice-message-component']") as IHtmlElement;
		
		public IHtmlLabelElement ContactEmailLablel =>
			(IHtmlLabelElement) Page.QuerySelector("#lblContactEmail");

		public IHtmlInputElement ContactEmailInput =>
			(IHtmlInputElement) Page.QuerySelector("#contactEmail");

		public IHtmlLabelElement PrimaryPhoneNumberLabel =>
			(IHtmlLabelElement) Page.QuerySelector("[data-testid='PrimaryPhoneNumber']");

		public IHtmlInputElement PrimaryPhoneNumberInput =>
			(IHtmlInputElement) Page.QuerySelector("#primaryPhoneNumber");

		public IHtmlLabelElement AlternativePhoneNumberLabel => Page.QuerySelector("[data-testid='AlternativePrimaryPhoneNumber']") as IHtmlLabelElement;

		public IHtmlInputElement AlternativePhoneNumberInput => Page.QuerySelector("#alternativePhoneNumber") as IHtmlInputElement;

		public IHtmlElement LoginEmail => Page.QuerySelector("[data-testid='loginEmail']") as IHtmlElement;

		public IHtmlButtonElement SaveChangesButton => Page.QuerySelector("#btnSaveContactDetails") as IHtmlButtonElement;

		public IElement SuccessMessage =>
			Page.QuerySelector("#successMessage");


	}
}