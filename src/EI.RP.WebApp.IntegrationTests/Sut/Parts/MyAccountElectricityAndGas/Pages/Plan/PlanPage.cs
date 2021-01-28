using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Plan
{
	internal class PlanPage : MyAccountElectricityAndGasPage
	{
		public PlanPage(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Plan"));
			}

			return isInPage;
		}

		public IHtmlElement Page => Document.QuerySelector("[data-page='plan']") as IHtmlElement;

		//PlanDetails component elements
		public IHtmlHeadingElement PricePlanHeading =>
			Page.QuerySelector("[data-testid='price-plan-heading']") as IHtmlHeadingElement;
		public IHtmlElement PricePlanLabel =>
			Page.QuerySelector("[data-testid='price-plan-label']") as IHtmlElement;
		public IHtmlElement PricePlan =>
			Page.QuerySelector("[data-testid='price-plan']") as IHtmlElement;
		public IHtmlElement PricePlanDiscountLabel =>
			Page.QuerySelector("[data-testid='price-plan-discount-label']") as IHtmlElement;
		public IHtmlElement PricePlanDiscount =>
			Page.QuerySelector("[data-testid='price-plan-discount']") as IHtmlElement;
		public IHtmlElement AddGas =>
			Page.QuerySelector("[data-testid='add-gas']") as IHtmlElement;
		public IHtmlAnchorElement AddGasFlow =>
			Page.QuerySelector("[data-testid='add-gas-flow']") as IHtmlAnchorElement;
		public IHtmlHeadingElement UpgradeToSmartHeading =>
			Page.QuerySelector("[data-testid='upgrade-to-smart-heading']") as IHtmlHeadingElement;
		public IHtmlElement UpgradeToSmartText =>
			Page.QuerySelector("[data-testid='upgrade-to-smart-text']") as IHtmlElement;
		public IHtmlAnchorElement UpgradeToSmartLink =>
			Page.QuerySelector("[data-testid='upgrade-to-smart-link']") as IHtmlAnchorElement;

		//AccountBilling component elements
		public IHtmlHeadingElement AccountBillingHeading =>
			Page.QuerySelector("[data-testid='billing-heading']") as IHtmlHeadingElement;
		public IHtmlElement PaymentMethod =>
			Page.QuerySelector("[data-testid='payment-method']") as IHtmlElement;
		public IHtmlElement PaymentMethodLabel =>
			Page.QuerySelector("[data-testid='payment-method-label']") as IHtmlElement;
		public IHtmlElement DirectDebitBank =>
			Page.QuerySelector("[data-testid='direct-debit-bank']") as IHtmlElement;
		public IHtmlElement DirectDebitBankLabel =>
			Page.QuerySelector("[data-testid='direct-debit-bank-label']") as IHtmlElement;
		public IHtmlElement DirectDebitIban =>
			Page.QuerySelector("[data-testid='direct-debit-iban']") as IHtmlElement;
		public IHtmlElement DirectDebitIbanLabel =>
			Page.QuerySelector("[data-testid='direct-debit-iban-label']") as IHtmlElement;
		public IHtmlAnchorElement EditDirectDebitLink =>
			Page.QuerySelector("[data-testid='edit-direct-debit-link']") as IHtmlAnchorElement;

		//Equalier
		public IHtmlHeadingElement EqualiserHeading =>
			Page.QuerySelector("[data-testid='equaliser-heading']") as IHtmlHeadingElement;
		public IHtmlElement EqualiserText =>
			Page.QuerySelector("[data-testid='equaliser-text']") as IHtmlElement;
		public IHtmlAnchorElement EqualiserLink =>
			Page.QuerySelector("[data-testid='equaliser-link']") as IHtmlAnchorElement;
		public IHtmlElement EqualiserDisabledLink =>
			Page.QuerySelector("[data-testid='equaliser-nolink']") as IHtmlElement;

		//Smart Meter data
		public IHtmlHeadingElement SmartMeterDataHeading =>
			Page.QuerySelector("[data-testid='smart-meter-data-heading']") as IHtmlHeadingElement;
		public IHtmlInputElement SmartMeterDataToggle =>
			Page.QuerySelector("[data-testid='smart-meter-data-toggle']") as IHtmlInputElement;
		public IHtmlElement SmartMeterDataText =>
			Page.QuerySelector("[data-testid='smart-meter-data-text']") as IHtmlElement;
		public IHtmlElement SmartMeterDataDowngradCheckBoxText =>
			Page.QuerySelector("[data-testid='meter-data-downgrade-checkbox-text']") as IHtmlElement;
		public IHtmlAnchorElement SmartMeterDataTermsAndConditionsLink =>
			Page.QuerySelector("[data-testid='meter-data-terms-and-conditions-link']") as IHtmlAnchorElement;

		//Paperless billing
		public IHtmlHeadingElement PaperlessBillingHeading =>
			Page.QuerySelector("[data-testid='paperless-billing-heading']") as IHtmlHeadingElement;
		public IHtmlInputElement PaperlessBillingToggle =>
			Page.QuerySelector("[data-testid='paperless-billing-toggle']") as IHtmlInputElement;				

		public bool? PaperlessBillSelected => !PaperlessBillingToggle?.IsChecked();
		public bool? PaperBillSelected => PaperlessBillingToggle?.IsChecked();

		public IHtmlElement PaperlessBillingText =>
			Page.QuerySelector("[data-testid='paperless-billing-text']") as IHtmlElement;
		public IHtmlAnchorElement PaperlessBillingLink =>
			Page.QuerySelector("[data-testid='paperless-billing-link']") as IHtmlAnchorElement;
		public IHtmlElement PaperlessBillingDisabledText =>
			Page.QuerySelector("[data-testid='paperless-billing-nolink']") as IHtmlElement;

		public IHtmlElement PaperBillingConfirmationDialog =>
			Page.QuerySelector("[data-testid='paper-billing-confirmation']") as IHtmlElement;
		public IHtmlHeadingElement PaperBillingConfirmationDialogHeading =>
			Page.QuerySelector("[data-testid='paper-billing-confirmation-heading']") as IHtmlHeadingElement;
		public IHtmlElement PaperBillingConfirmationDialogText =>
			Page.QuerySelector("[data-testid='paper-billing-confirmation-text']") as IHtmlElement;
		public IHtmlButtonElement PaperBillingConfirmationDialogYesButton =>
			Page.QuerySelector("[data-testid='paper-billing-confirmation-yes-button']") as IHtmlButtonElement;
		public IHtmlButtonElement PaperBillingConfirmationDialogNoButton =>
			Page.QuerySelector("[data-testid='paper-billing-confirmation-no-button']") as IHtmlButtonElement;

		//Monthly Billing
		public IHtmlHeadingElement MonthlyBillingHeading => Page.QuerySelector("[data-testid='monthly-billing-heading']") as IHtmlHeadingElement;
		public IHtmlInputElement MonthlyBillingToggle => Page.QuerySelector("[data-testid='monthly-billing-toggle']") as IHtmlInputElement;

		public IHtmlElement MonthlyBillChangeModal => Page.QuerySelector("[data-testid='monthly-bill-change-modal']") as IHtmlElement;
		public IHtmlHeadingElement MonthlyBillChangeModalHeading => Page.QuerySelector("[data-testid='monthly-bill-change-modal-heading']") as IHtmlHeadingElement;
		public IHtmlLabelElement MonthlyBillChangeModalLabel => Page.QuerySelector("[data-testid='monthly-bill-change-modal-label']") as IHtmlLabelElement;
		public IHtmlElement MonthlyBillChangeModalText => Page.QuerySelector("[data-testid='monthly-bill-change-modal-text']") as IHtmlElement;
		public IHtmlSelectElement MonthlyBillChangeModalDates => Page.QuerySelector("[data-testid='monthly-bill-change-modal-dates']") as IHtmlSelectElement;
		public IHtmlButtonElement MonthlyBillChangeModalContinue => Page.QuerySelector("[data-testid='monthly-bill-change-modal-continue']") as IHtmlButtonElement;
		public IHtmlButtonElement MonthlyBillChangeModalCancel => Page.QuerySelector("[data-testid='monthly-bill-change-modal-cancel']") as IHtmlButtonElement;

		public IHtmlElement CancelMonthlyBillModal => Page.QuerySelector("[data-testid='cancel-monthly-billing-modal']") as IHtmlElement;
		public IHtmlHeadingElement CancelMonthlyBillModalHeading => Page.QuerySelector("[data-testid='cancel-monthly-billing-modal-heading']") as IHtmlHeadingElement;
		public IHtmlElement CancelMonthlyBillModalText => Page.QuerySelector("[data-testid='cancel-monthly-billing-modal-text']") as IHtmlElement;
		public IHtmlButtonElement CancelMonthlyBillModalContinue => Page.QuerySelector("[data-testid='cancel-monthly-billing-modal-continue']") as IHtmlButtonElement;
		public IHtmlButtonElement CancelMonthlyBillModalCancel => Page.QuerySelector("[data-testid='cancel-monthly-billing-modal-cancel']") as IHtmlButtonElement;

		public IHtmlElement MonthlyBillText => Page.QuerySelector("[data-testid='monthly-bill-text']") as IHtmlElement;
		public IHtmlElement BiMonthlyBillText => Page.QuerySelector("[data-testid='bimonthly-bill-text']") as IHtmlElement;
		public IHtmlElement MonthlyBillChangeDate => Page.QuerySelector("[data-testid='monthly-bill-change-date']") as IHtmlElement;

	}
}
