using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.ChangePassword
{
	internal class ChangePasswordPage : MyAccountElectricityAndGasPage
	{
		public ChangePasswordPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement Page =>
			(IHtmlElement) Document.QuerySelector("[data-page='change-password']");

		public IHtmlElement ErrorMessage =>
			(IHtmlElement)Page.QuerySelector("[data-testid='change-password-error']");

		public IHtmlInputElement CurrentPassword =>
			(IHtmlInputElement) Page.QuerySelector("#currentPassword");

		public IHtmlInputElement NewPassword =>
			(IHtmlInputElement) Page.QuerySelector("#newPassword");

		public IHtmlButtonElement SaveNewPasswordBtn =>
			(IHtmlButtonElement) Page.QuerySelector("#dds_update_details_btn");

		protected override bool IsInPage()
		{
			var isInPage = Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Change Password"));
			}

			return isInPage;
		}
	}
}