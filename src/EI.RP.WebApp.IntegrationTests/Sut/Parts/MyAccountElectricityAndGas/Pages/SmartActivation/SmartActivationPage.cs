using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation
{
	internal abstract class SmartActivationPage : MyAccountElectricityAndGasPage
	{
		protected SmartActivationPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlAnchorElement CancelButton => Document.QuerySelector("[data-testid='modal-cancel-smart-activation-yes']") as IHtmlAnchorElement;

		public IHtmlElement Stepper => Document.QuerySelector("[data-testid='smart-activation-stepper']") as IHtmlElement;
	}
}