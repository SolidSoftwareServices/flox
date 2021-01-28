using System.Linq;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration.
	DirectDebit
{
	internal class ConfigureDirectDebitPage : MyAccountElectricityAndGasPage
	{
		private static readonly string[] ExpectedValues = {"Complete Direct Debit Setup"};

		public ConfigureDirectDebitPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlHeadingElement Heading => (IHtmlHeadingElement) Document.QuerySelector(".portal-details > h2");

		public IHtmlButtonElement UpdateDetailsButton => (IHtmlButtonElement) Document
			.QuerySelector("#directdebit button:not([type='button'])");

		public IHtmlInputElement Iban =>
			(IHtmlInputElement) Document.QuerySelector("#iban");

		public IHtmlInputElement IbanError =>
			(IHtmlInputElement) Document.QuerySelector("#iban-error");

		public IHtmlInputElement CustomerName =>
			(IHtmlInputElement) Document.QuerySelector("#customer-name");

		public IHtmlElement SaveMoneyReminder =>
			(IHtmlElement) Document.QuerySelector("#content > p:nth-child(2)");

		public IHtmlElement PrivacyNotice =>
			(IHtmlElement) Document.QuerySelector("#gdprText");

		public IHtmlInputElement ConfirmTermsBox =>
			(IHtmlInputElement) Document.QuerySelector("#terms");

		public IHtmlInputElement CheckBoxInput =>
			(IHtmlInputElement) Document.QuerySelector("#BillsAndPayments- > input:nth-child(8)");

		public IHtmlSpanElement UnconfirmErrorMessage =>
			(IHtmlSpanElement) Document.QuerySelector("#directdebit > div > span");

		protected override bool IsInPage()
		{
			return base.IsInPage()
			       && Heading?.TextContent == "Direct Debit Settings - Gas"
			       && ExpectedValues.Any(x => x == UpdateDetailsButton?.TextContent)
			       && Iban != null;
		}

		public IHtmlButtonElement CompleteDirectDebitButton()
		{
			var result = (IHtmlButtonElement) Document
				.QuerySelector("#directdebit button:not([type='button'])");
			return result;
		}

		public IHtmlAnchorElement CancelBtn()
		{
			var result = (IHtmlAnchorElement) Document
				.QuerySelector("#directdebit > a");
			return result;
		}

		public IHtmlButtonElement SureBtn()
		{
			var result = (IHtmlButtonElement) Document
				.QuerySelector("body > div > div > div.popup_content.popup_content--wide > button");
			return result;
		}

		public ConfigureDirectDebitPage InputFormValues(string iban, string accountName)
		{
			Iban.Value = iban;
			CustomerName.Value = "Account Name";
			ConfirmTermsBox.IsChecked = true;
			return this;
		}
	}
}