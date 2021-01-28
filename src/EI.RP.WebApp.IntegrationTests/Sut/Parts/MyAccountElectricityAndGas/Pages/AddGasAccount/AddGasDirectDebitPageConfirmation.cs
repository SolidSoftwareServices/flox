using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AddGasAccount
{
	internal class AddGasDirectDebitPageConfirmation : MyAccountElectricityAndGasPage
	{
		public AddGasDirectDebitPageConfirmation(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement Page =>
			Document.QuerySelector("[data-page='add-gas-confirmation']") as IHtmlElement;

		public IHtmlHeadingElement Heading =>
			Document.QuerySelector("[data-testid='add-gas-confirmation-title']") as IHtmlHeadingElement;

		protected override bool IsInPage()
		{
			var isInPage = Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Add Gas"));
			}

			return isInPage;
		}
	}
}