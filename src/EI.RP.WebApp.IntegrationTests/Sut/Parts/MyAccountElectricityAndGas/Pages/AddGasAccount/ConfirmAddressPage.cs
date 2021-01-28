using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AddGasAccount
{
	internal class ConfirmAddressPage : CollectAccountConsumptionDetailsPage
	{
		public ConfirmAddressPage(ResidentialPortalApp app) : base(app)
		{
		}

		public override IHtmlElement Page =>
			Document.QuerySelector("[data-page='add-gas-confirm-address']") as IHtmlElement;

		public IHtmlButtonElement ConfirmAddressButton => Document
			.QuerySelector("#btnConfirmAddress") as IHtmlButtonElement;

		public IHtmlElement GPRNHeading => Document
			.QuerySelector("#gprn > strong") as IHtmlElement;

		public IHtmlElement Content => Document
			.QuerySelector("#confirmAddressText") as IHtmlElement;

		public IHtmlAnchorElement NotMyAddressButton => Document
			.QuerySelector("#btnNotMyAddress") as IHtmlAnchorElement;

		public IHtmlElement NeedHelpContent => Document
			.QuerySelector("#help-number") as IHtmlElement;

		public IHtmlElement Address => Document
			.QuerySelector("#address-details") as IHtmlElement;

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