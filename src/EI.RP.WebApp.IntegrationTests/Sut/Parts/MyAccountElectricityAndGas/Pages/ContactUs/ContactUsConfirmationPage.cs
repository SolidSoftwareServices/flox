using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.ContactUs
{
	internal class ContactUsConfirmationPage : ContactUsPage
	{
		public ContactUsConfirmationPage(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			var isInPage = Document.QuerySelector("[data-page='contact-us-confirmation']") != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Contact Us"));
			}

			return isInPage;
		}
	}
}