using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation
{
	internal class Step3PaymentDetailsPage : SmartActivationPage
	{
		public Step3PaymentDetailsPage(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Page != null;

			if (isInPage)
			{
				SetupDirectDebitElementForManualPayment = new SetupDirectDebit(Document);
				ExistingDirectDebitElement = new ExistingDirectDebit(Document);
				SetupNewDirectDebitPopupElement = new SetupNewDirectDebitPopup(Document);
				AlternativePayerElement = new AlternativePayer(Document);
				AssertTitle(App.ResolveTitle("3. Payment Details | Smart sign up"));
			}

			return isInPage;
		}

		public IHtmlElement Page => (IHtmlElement)Document.QuerySelector("[data-page='step3-payment-details']");
		public IHtmlHeadingElement PaymentHeading => Document.QuerySelector("[data-testid='smart-activation-payment-heading']") as IHtmlHeadingElement;
		
		public ExistingDirectDebit ExistingDirectDebitElement { get; private set; }
		public SetupDirectDebit SetupDirectDebitElementForManualPayment { get; private set; }
		public SetupNewDirectDebitPopup SetupNewDirectDebitPopupElement { get; private set; }
		public AlternativePayer AlternativePayerElement { get; private set; }

		public class SetupDirectDebit
		{
			private IHtmlDocument _document;

			public SetupDirectDebit(IHtmlDocument document)
			{
				_document = document;
			}
			public IHtmlElement SetupNewDirectDebitOption => _document.QuerySelector("[data-testid='setup-new-direct-debit-option']") as IHtmlElement;
			public IHtmlInputElement NameInput => SetupNewDirectDebitOption.QuerySelector("#customer-name") as IHtmlInputElement;
			public IHtmlElement NameInputError => SetupNewDirectDebitOption.QuerySelector("#customer-name-error") as IHtmlElement;
			public IHtmlInputElement IbanInput => SetupNewDirectDebitOption.QuerySelector("#iban") as IHtmlInputElement;
			public IHtmlElement IbanError => SetupNewDirectDebitOption.QuerySelector("#iban-error") as IHtmlElement;
			public IHtmlButtonElement AddDebitCardButton => SetupNewDirectDebitOption.QuerySelector("#addDebitCard") as IHtmlButtonElement;
			public IHtmlButtonElement SkipButton => SetupNewDirectDebitOption.QuerySelector("#skipDirectDebit") as IHtmlButtonElement;
			public IHtmlAnchorElement SetupNewDirectDebitLink => SetupNewDirectDebitOption.QuerySelector("[data-testid='setup-new-direct-debit']") as IHtmlAnchorElement;
		}

		public class ExistingDirectDebit
		{
			private readonly IHtmlDocument _document;

			public ExistingDirectDebit(IHtmlDocument document)
			{
				_document = document;
			}

			public IHtmlElement ExistingDirectDebitOption => _document.QuerySelector("[data-testid='existing-dd-card']") as IHtmlElement;
			public IHtmlElement NameOnBank => ExistingDirectDebitOption.QuerySelector("[data-testid='name-on-account']") as IHtmlElement;
			public IHtmlElement ExistingIban => ExistingDirectDebitOption.QuerySelector("[data-testid='iban']") as IHtmlElement;
			public IHtmlInputElement ConfirmUseExistingDebit => ExistingDirectDebitOption.QuerySelector("#confirmContinueDebit") as IHtmlInputElement;
			public IHtmlLabelElement ConfirmUseExistingDebitLabel => ExistingDirectDebitOption.QuerySelector("#lblConfirmContinueDebit") as IHtmlLabelElement;
			public IHtmlElement ConfirmUseExistingDebitError => ExistingDirectDebitOption.QuerySelector("#confirmContinueDebit-error") as IHtmlElement;
			public IHtmlButtonElement UseExistingDirectDebitButton => ExistingDirectDebitOption.QuerySelector("#useExistingDirectDebit") as IHtmlButtonElement;
		}

		public class SetupNewDirectDebitPopup
		{
			private readonly IHtmlDocument _document;

			public SetupNewDirectDebitPopup(IHtmlDocument document)
			{
				_document = document;
			}

			public IHtmlElement Popup => _document.QuerySelector("[data-testid='setup-new-dd-popup']") as IHtmlElement;
			public IHtmlInputElement NameInput => Popup.QuerySelector("#customer-name") as IHtmlInputElement;
			public IHtmlElement NameInputError => Popup.QuerySelector("#customer-name-error") as IHtmlElement;
			public IHtmlInputElement IbanInput => Popup.QuerySelector("#iban") as IHtmlInputElement;
			public IHtmlElement IbanError => Popup.QuerySelector("#iban-error") as IHtmlElement;
			public IHtmlButtonElement AddDebitCardButton => Popup.QuerySelector("#addDebitCard") as IHtmlButtonElement;
		}

		public class AlternativePayer
		{
			private readonly IHtmlDocument _document;

			public AlternativePayer(IHtmlDocument document)
			{
				_document = document;
			}
			public IHtmlElement MainElement => _document.QuerySelector("[data-testid='alternative-payer']") as IHtmlElement;
			public IHtmlElement Message => _document.QuerySelector("[data-testid='alternative-payer-message']") as IHtmlElement;
			public IHtmlElement QueryMessage => _document.QuerySelector("[data-testid='alternative-payer-query-message']") as IHtmlElement;
			public IHtmlButtonElement ContinueButton => _document.QuerySelector("#continueAlternativePayer") as IHtmlButtonElement;
		}
	}
}