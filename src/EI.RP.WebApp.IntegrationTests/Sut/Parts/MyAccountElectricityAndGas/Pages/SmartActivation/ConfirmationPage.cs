using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation
{
	internal class ConfirmationPage : SmartActivationPage
	{
		public ConfirmationPage(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Confirmation | Smart sign up"));
			}

			return isInPage;
		}

		public IHtmlElement Page => (IHtmlElement)Document.QuerySelector("[data-page='smart-activation-confirmation']");
		public IHtmlHeadingElement PageHeading => Page.QuerySelector("[data-testid='confirmation-heading']") as IHtmlHeadingElement;
		public IHtmlElement ThankYouWithSelectedPlan => Page.QuerySelector("[data-testid='thank-you-with-selected-plan-element']") as IHtmlElement;
		public IHtmlElement WeAreDelightedElement => Page.QuerySelector("[data-testid='we-are-delighted-element']") as IHtmlElement;
		public IHtmlElement YouWillShortlyElement => Page.QuerySelector("[data-testid='you-will-shortly-element']") as IHtmlElement;
		public IHtmlAnchorElement BackToAccountsLink =>
					(IHtmlAnchorElement)Page.QuerySelector("[data-testid='back_to_my_accounts_from_smart_activation']");

	}
}