using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse
{
	internal class Step5ConfirmationPage : MyAccountElectricityAndGasPage
	{
		public Step5ConfirmationPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement Step5ConfirmationTempScreen =>
			Document.QuerySelector("[data-page='mimo-5-confirm']") as IHtmlElement;

		public IHtmlElement ConfirmationElectricityAccountNumber =>
			Document.QuerySelector("[data-id='h3AccountNumberE']") as IHtmlElement;

		public IHtmlElement ConfirmationGasAccountNumber =>
			Document.QuerySelector("[data-id='h3AccountNumberG']") as IHtmlElement;

		public IHtmlParagraphElement FreeElectricityAllowanceNotice =>
			Document.QuerySelector("[data-id='pFreeElectricityAllowanceNotice']") as IHtmlParagraphElement;

		public IHtmlElement DirectDebitChangeConfirmation =>
			Document.QuerySelector("[data-testid='direct-debit-change-confirmation']") as IHtmlElement;

		public IHtmlElement DirectDebitBillMessage =>
			Document.QuerySelector("[data-testid='direct-debit-bill']") as IHtmlElement;

		public IHtmlAnchorElement BillAndPaymentLink =>
			Document.QuerySelector("[data-testid='direct-debit-billing-and-payment']") as IHtmlAnchorElement;

		public IHtmlAnchorElement BackToMyAccountsLink =>
			Document.QuerySelector("[data-id='aMyAccounts']") as IHtmlAnchorElement;

		public IHtmlHeadingElement ThankYouAccountMoveNotice =>
			Document.QuerySelector("[data-id='h2ThankYouAccountMove']") as IHtmlHeadingElement;

		public IHtmlSpanElement CreatingNewAccountsNotice =>
			Document.QuerySelector("[data-id='sCreatingNewAccountsNotice']") as IHtmlSpanElement;

		public IHtmlParagraphElement YourNewSavingsWillBeAppliedNotice =>
			Document.QuerySelector("[data-id='p2YourNewSavingsWillBeAppliedNotice']") as IHtmlParagraphElement;

		public IHtmlParagraphElement YourElectricityAccountWillBePaidByNotice =>
			Document.QuerySelector("[data-id='pYourElectricityAccountWillBePaidByNotice']") as IHtmlParagraphElement;

		public IHtmlParagraphElement YourGasAccountWillBePaidByNotice =>
			Document.QuerySelector("[data-id='pYourGasAccountWillBePaidByNotice']") as IHtmlParagraphElement;

		public IHtmlAnchorElement ElectricityAndGasPricing =>
			Document.QuerySelector("[data-id='aElectricityAndGasPricing']") as IHtmlAnchorElement;

		public IHtmlAnchorElement ReadTermsAndConditions =>
			Document.QuerySelector("[data-id='aReadTermsAndConditions']") as IHtmlAnchorElement;

		protected override bool IsInPage()
		{
			var isInPage = Step5ConfirmationTempScreen != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Confirmation | Moving House"));
			}

			return isInPage;
		}
	}
}