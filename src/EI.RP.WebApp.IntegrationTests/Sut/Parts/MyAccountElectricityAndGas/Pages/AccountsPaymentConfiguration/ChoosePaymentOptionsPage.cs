using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration
{
	internal class ChoosePaymentOptionsPage : MyAccountElectricityAndGasPage
	{
		public ChoosePaymentOptionsPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement Subtitle => Document.QuerySelector("#choosePaymentOptions") as IHtmlElement;

		public IHtmlButtonElement UseExistingDirectDebitButton => Document
			.QuerySelector("#useExistingDirectDebit") as IHtmlButtonElement;

		public IHtmlButtonElement SetUpNewDirectDebitButton => Document
			.QuerySelector("#setUpNewDirectDebit") as IHtmlButtonElement;

		public IHtmlButtonElement SetUpNewDirectDebitDialogButton => Document
			.QuerySelector("#setUpNewDirectDebitDlg") as IHtmlButtonElement;

		public IHtmlElement SetUpNewDirectDebitHeader => Document
			.QuerySelector("[data-testid='payment-option-new-dd-title']") as IHtmlElement;

		public IHtmlElement SetUpNewDirectDebitContent => Document
			.QuerySelector("[data-testid='payment-option-new-dd-text']") as IHtmlElement;

		public IHtmlButtonElement ManualPaymentsButton => Document
			.QuerySelector("#manualPaymentDlg") as IHtmlButtonElement;

		public IHtmlHeadingElement ManualPaymentsHeader => Document
			.QuerySelector("[data-testid='payment-option-manual-title']") as IHtmlHeadingElement;

		public IHtmlButtonElement UseUpExistingDirectDebitButton => Document
			.QuerySelector("#useExistingDirectDebit") as IHtmlButtonElement;

		public IHtmlHeadingElement UseUpExistingDirectDebitHeader => Document
			.QuerySelector("#useExistingDirectDebitHeader") as IHtmlHeadingElement;

		public IHtmlElement ExistingDirectDebitDivContent => Document
			.QuerySelector("#existingDirectDebitDivContent") as IHtmlElement;

		public IHtmlInputElement CheckBoxUseExistingDDSetupForAllAccounts => Document
			.QuerySelector("#useExistingSingleSetupForAllAccounts") as IHtmlInputElement;

		public IHtmlLabelElement LabelUseExistingDDSetupForAllAccounts => Document
			.QuerySelector("#lblUseExistingSingleSetupForAllAccounts") as IHtmlLabelElement;

		public IHtmlInputElement CheckBoxConfirmContinueDebit => Document
			.QuerySelector("#confirmContinueDebit") as IHtmlInputElement;

		public IHtmlLabelElement LabelConfirmContinueDebit => Document
			.QuerySelector("#lblConfirmContinueDebit") as IHtmlLabelElement;

		public IHtmlElement ManualPaymentsContent => Document
			.QuerySelector("[data-testid='payment-option-manual-text']") as IHtmlElement;

		public IHtmlAnchorElement GoBack => Document
			.QuerySelector("#btnGoBack") as IHtmlAnchorElement;

		public IHtmlAnchorElement SkipAndCompleteButton => Document
			.QuerySelector("#btnSkipAndComplete") as IHtmlAnchorElement;

		public IHtmlAnchorElement CancelButton => Document
			.QuerySelector("#btnCancel") as IHtmlAnchorElement;

		public IHtmlButtonElement ConfirmCancelButton => Document
			.QuerySelector("#cancelFlowConfirm") as IHtmlButtonElement;

		public IHtmlElement UseExistingDirectDebitConfirmationNotCheckedErrorMessage =>
			Document.QuerySelector("#continueDebit .alert.alert-form.alert-danger") as IHtmlElement;

		public IHtmlInputElement ConfirmContinueDebit =>
			Document.QuerySelector("#confirmContinueDebit") as IHtmlInputElement;

		public IHtmlElement FirstOptionHeader =>
			Document.QuerySelectorAll("[data-testid='payment-options-container']").First()
				.QuerySelector("h5") as IHtmlElement;

		public IHtmlInputElement UseSameNewDirectDebitForAllAccountsCheckBox => Document
			.QuerySelector("#useNewSingleSetupForAllAccounts") as IHtmlInputElement;

		public IHtmlInputElement UseNewSingleSetupForAllAccountsFromManualConfirmDialog => Document
			.QuerySelector("#useNewSingleSetupForAllAccountsFromManualConfirmDialog") as IHtmlInputElement;

		public IHtmlInputElement ConfirmDetailsAreCorrectExistingDirectDebitCheckBox =>
			Document.QuerySelector("#confirmContinueDebit") as IHtmlInputElement;

		public IHtmlLabelElement UseSameForAllAccountsLabel => Document
			.QuerySelector("#lblUseSameSetupForAllAccounts") as IHtmlLabelElement;

		public IHtmlInputElement UseExistingDDSetupForAllAccountsCheckBox => Document
			.QuerySelector("#useExistingSingleSetupForAllAccounts") as IHtmlInputElement;

		protected override bool IsInPage()
		{
			return Subtitle?.TextContent?.Trim() == "Choose a payment option:";
		}

		public async Task<ResidentialPortalApp> ClickOnCancelConfirm()
		{
			return await ClickOnElement(ConfirmCancelButton) as ResidentialPortalApp;
		}

		public async Task<ResidentialPortalApp> SelectManualPayment()
		{
			return await ClickOnElement(ManualPaymentsButton) as ResidentialPortalApp;
		}

		public async Task<ResidentialPortalApp> SelectNewDirectDebitFromDialog()
		{
			return await ClickOnElement(SetUpNewDirectDebitDialogButton) as ResidentialPortalApp;
		}

		public async Task<ResidentialPortalApp> SelectNewDirectDebit()
		{
			return await ClickOnElement(SetUpNewDirectDebitButton) as ResidentialPortalApp;
		}

		public async Task<ResidentialPortalApp> SelectExistingDirectDebit()
		{
			return await ClickOnElement(UseExistingDirectDebitButton) as ResidentialPortalApp;
		}
	}
}