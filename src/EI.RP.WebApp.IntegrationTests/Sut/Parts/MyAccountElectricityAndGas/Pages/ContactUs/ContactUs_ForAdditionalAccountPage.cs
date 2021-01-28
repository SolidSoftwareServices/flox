using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.ContactUs
{
	internal class ContactUs_ForAdditionalAccountPage : ContactUsPage
	{
		public ContactUs_ForAdditionalAccountPage(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage()
			               && Document.QuerySelector("#addAcc > h2")?.TextContent?.Trim() == "Enter Account Details";

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Contact Us"));
			}

			return isInPage;
		}
	}
}