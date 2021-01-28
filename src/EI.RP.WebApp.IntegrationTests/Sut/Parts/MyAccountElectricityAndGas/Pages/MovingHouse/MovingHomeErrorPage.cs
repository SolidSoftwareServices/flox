using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse
{
	internal class MovingHomeErrorPage : MyAccountElectricityAndGasPage
	{
		public MovingHomeErrorPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement ErrorMessage => Document.QuerySelector("#errorMessage") as IHtmlElement;

		public IHtmlElement ContactNumber => Document.QuerySelector("#contactNumber") as IHtmlElement;

		public IHtmlAnchorElement BackToAccounts =>
			Document.QuerySelector("[data-testid='back-to-my-accounts']") as IHtmlAnchorElement;

		protected override bool IsInPage()
		{
			var result = base.IsInPage();
			var stepHeader = (IHtmlHeadingElement) Document.QuerySelector("#header");
			if (result)
			{
				var isInPage = stepHeader?.TextContent == "Moving Electricity Account"
				               || stepHeader?.TextContent == "Moving Electricity & Adding Gas Account" 
				               || stepHeader?.TextContent == "Closing Electricity Account" 
				               || stepHeader?.TextContent == "Moving Electricity & Closing Gas Account" 
				               || stepHeader?.TextContent == "Closing Gas Accounts" 
				               || stepHeader?.TextContent == "Closing Electricity & Gas Accounts" 
				               || stepHeader?.TextContent == "Moving Gas & Adding Electricity Account" 
				               || stepHeader?.TextContent == "Moving Electricity & Gas Accounts" 
				               || stepHeader?.TextContent == "Moving Gas Account";

				if (isInPage)
				{
					AssertTitle(App.ResolveTitle("Moving House"));
				}

				return isInPage;
			}

			return false;
		}
	}
}