using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.MovingHouse
{
	internal class MovingHouseUnhandledErrorPage : MyAccountElectricityAndGasPage
	{
		public MovingHouseUnhandledErrorPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement Page => Document.QuerySelector("[data-page='error_page']") as IHtmlElement;

        public IHtmlElement Title => Page.QuerySelector("[data-testid='title']") as IHtmlElement;

        public IHtmlElement Subtitle => Page.QuerySelector("[data-testid='subtitle']") as IHtmlElement;

        public IHtmlElement BackToAccounts => Page.QuerySelector("#backToMyAccount") as IHtmlElement;

		public IHtmlElement ContactNumber => Page.QuerySelector("#contactNumber") as IHtmlElement;

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Moving House"));
			}

			return isInPage;
		}
	}
}