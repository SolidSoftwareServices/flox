using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.Flows.AppFlows.AccountsPaymentConfiguration.Steps;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration
{
	internal class InputDirectDebitDetailsWhenMovingHomePage : MyAccountElectricityAndGasPage
	{
		//TODO: TO BE DONE AND MERGED FROM InputDirectDebitDetailsPage TO HAVE ONE FOR ALL
		public InputDirectDebitDetailsWhenMovingHomePage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlButtonElement ManualPaymentConfirmButton =>
			Document.QuerySelector("#manualPaymentConfirmButton") as IHtmlButtonElement;

		public IHtmlButtonElement CancelDirectDebitSetupYesImSureButton =>
			Document.QuerySelector("#cancelDirectDebitSetupYesImSure") as IHtmlButtonElement;

		public IHtmlInputElement Iban => Document.QuerySelector("#iban") as IHtmlInputElement;
		public IHtmlElement IbanErrorMessage => Document.QuerySelector("#iban-error") as IHtmlElement;
		public IHtmlInputElement CustomerName => Document.QuerySelector("#customer-name") as IHtmlInputElement;
		public IHtmlElement CustomerNameErrorMessage => Document.QuerySelector("#customer-name-error") as IHtmlElement;
		public IHtmlInputElement TermsAndConditions => Document.QuerySelector("#terms") as IHtmlInputElement;
		public IHtmlElement TermsAndConditionsErrorMessage => Document.QuerySelector("#terms-error") as IHtmlElement;

		public IHtmlButtonElement CompleteDirectDebitSetup =>
			Document.QuerySelector($"button[value='{InputDirectDebitDetails.StepEvent.DirectDebitSetupCompleted}']") as
				IHtmlButtonElement;

		public IHtmlElement SkipElement => Document.QuerySelector("#manualPaymentConfirmButton") as IHtmlElement;

		public IHtmlHeadingElement HeaderElement =>
			Document.QuerySelector("#inputDirectDebitHeader") as IHtmlHeadingElement;

		public IHtmlElement SkipDirectDebitSetupLink =>
			Document.QuerySelector("a[data-target='#modal-skip-dd-setup']") as IHtmlElement;

		public IHtmlElement CancelDirectDebitSetupLink =>
			Document.QuerySelector("a[data-target='#modal-cancel-dd-setup']") as IHtmlElement;

		public IHtmlInputElement ConfirmTerms => (IHtmlInputElement) Document.QuerySelector("#terms");

		protected override bool IsInPage()
		{
			var isInPage = Document.QuerySelector("#step4.active") != null && Iban != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("4. Payment | Moving House"));
			}

			return isInPage;
		}

		public InputDirectDebitDetailsWhenMovingHomePage InputFormValues(string iban = "IE65AIBK93104715784037",
			string accountName = "Account Name", bool confirmTerms = true)
		{
			Iban.Value = iban;
			CustomerName.Value = accountName;
			ConfirmTerms.IsChecked = confirmTerms;
			return this;
		}

		public async Task<ResidentialPortalApp> SelectManual()
		{
			await ClickOnElement(ManualPaymentConfirmButton);
			return App;
		}

		public async Task<ResidentialPortalApp> Complete()
		{
			await ClickOnElement(CompleteDirectDebitSetup);
			return App;
		}

		public async Task<ResidentialPortalApp> CancelDirectDebitSetup()
		{
			await ClickOnElement(CancelDirectDebitSetupYesImSureButton);
			return App;
		}

		public async Task<ResidentialPortalApp> Skip()
		{
			await ClickOnElement(SkipElement);
			return App;
		}

		public void AssertCancelDirectDebitAndSkipSetupLinks(bool shouldShowCancelLink, bool shouldShowSkipLink)
		{
			Assert.IsTrue(
				shouldShowCancelLink ? CancelDirectDebitSetupLink != null : CancelDirectDebitSetupLink == null);
			Assert.IsTrue(shouldShowSkipLink ? SkipDirectDebitSetupLink != null : SkipDirectDebitSetupLink == null);
		}
	}
}