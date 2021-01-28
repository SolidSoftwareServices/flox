using System.Linq;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration.
	DirectDebit
{
	internal class InputDirectDebitDetailsPage : MyAccountElectricityAndGasPage
	{
		private static readonly string[] ExpectedValues = {"Complete Direct Debit Setup", "Update details"};

		public InputDirectDebitDetailsPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlHeadingElement Heading => (IHtmlHeadingElement) Document.QuerySelector(".portal-details > h2");

		public IHtmlButtonElement UpdateDetailsButton =>
			(IHtmlButtonElement) Document.QuerySelector("[data-testid='dd-settings-primary-button']");

		public IHtmlInputElement Iban =>
			(IHtmlInputElement) Document.QuerySelector("#iban");

		public IHtmlSpanElement IbanError =>
			(IHtmlSpanElement) Document.QuerySelector("#iban-error");

		public IHtmlHeadingElement EditDirectDebitHeader =>
			(IHtmlHeadingElement) Document.QuerySelector(".portal-details > h2");

		public IHtmlInputElement CustomerName =>
			(IHtmlInputElement) Document.QuerySelector("#customer-name");

		public IHtmlElement SaveMoneyReminder =>
			(IHtmlElement) Document.QuerySelector(".portal-details > p");

		public IHtmlElement PrivacyNotice =>
			(IHtmlElement) Document.QuerySelector("[data-testid='privacy-notice-message-component'] > p");

		public IHtmlInputElement ConfirmTerms =>
			(IHtmlInputElement) Document.QuerySelector("#terms");

		public IHtmlInputElement CheckBoxInput =>
			(IHtmlInputElement) Document.QuerySelector("#BillsAndPayments- > input:nth-child(8)");

		public IHtmlSpanElement UnconfirmErrorMessage =>
			(IHtmlSpanElement) Document.QuerySelector("#directdebit > div > span");

		public IHtmlButtonElement CompleteDirectDebitButton =>
			(IHtmlButtonElement) Document.QuerySelector("[data-testid='dd-settings-primary-button']");

		public IHtmlAnchorElement CancelBtn =>
			(IHtmlAnchorElement) Document.QuerySelector("[data-testid='dd-settings-secondary-button']");

		public IHtmlAnchorElement SureBtn =>
			(IHtmlAnchorElement) Document.QuerySelector("[data-testid='modal-cancel-yes-i-am-sure']");

		protected override bool IsInPage()
		{
			return Heading?.TextContent == "Direct Debit Settings - Electricity"
			       && ExpectedValues.Any(x => x == UpdateDetailsButton?.TextContent);
		}
	}
}