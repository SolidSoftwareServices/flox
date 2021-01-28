using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Sut;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation
{
	internal class Step5SummaryPage : SmartActivationPage
	{
		public Step5SummaryPage(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("5. Summary | Smart sign up"));
			}

			return isInPage;
		}

		public IHtmlElement Page => (IHtmlElement)Document.QuerySelector("[data-page='step5-summary']");
		public IHtmlHeadingElement PageHeading => Page.QuerySelector("[data-testid='summary-heading']") as IHtmlHeadingElement;
		public IHtmlElement SelectedPlan => Page.QuerySelector("[data-testid='selected-plan']") as IHtmlElement;
		public IHtmlElement PaymentMethod => Page.QuerySelector("[data-testid='payment-method']") as IHtmlElement;
		public IHtmlElement BillFrequency => Page.QuerySelector("[data-testid='bill-frequency']") as IHtmlElement;
		public IHtmlButtonElement ContinueButton => Page.QuerySelector("#continueButton") as IHtmlButtonElement;

		public IHtmlInputElement TermsAndConditionsAcceptedCheckbox => Page.QuerySelector("#termsAndConditionsAccepted") as IHtmlInputElement;

		public IHtmlElement TermsAndConditionsAcceptedCheckboxError => Page.QuerySelector("[data-testid='terms-and-conditions-accepted-validation']") as IHtmlElement;

		public async Task<ISutApp> NextPage(bool termsAndConditionsAccepted)
		{
			TermsAndConditionsAcceptedCheckbox.IsChecked = termsAndConditionsAccepted;
			return await ClickOnElement(ContinueButton);
		}
	}
}