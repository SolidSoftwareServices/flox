using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.Usage
{
	internal class UsagePage : MyAccountElectricityAndGasPage
	{
		public UsagePage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement Page =>
			Document.QuerySelector("[data-page='usage']") as IHtmlElement;

		public IHtmlElement UsageChartComponent =>
			Page.QuerySelector("[data-testid='usage-chart-component']") as IHtmlElement;

		public IHtmlElement ComparisonModal =>
			Page.QuerySelector("[data-testid='usage-chart-comparison-modal']") as IHtmlElement;


		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Usage"));
			}

			return isInPage;
		}
	}
}