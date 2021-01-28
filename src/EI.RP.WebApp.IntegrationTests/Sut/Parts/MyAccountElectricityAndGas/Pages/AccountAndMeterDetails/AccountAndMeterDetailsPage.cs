using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountAndMeterDetails
{
	internal class AccountAndMeterDetailsPage : MyAccountElectricityAndGasPage
	{
		public AccountAndMeterDetailsPage(ResidentialPortalApp app) : base(app)
		{
		}

		public AccountAndMeterDetailsInfo DetailsInfo { get; private set; }

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() &&
			               Document.QuerySelector("[data-page='details']") != null;

			if (isInPage)
			{
				DetailsInfo = new AccountAndMeterDetailsInfo(Document);
			}

			var isInPageWithDetails = isInPage && DetailsInfo.Page != null;

			if (isInPageWithDetails)
			{
				AssertTitle(App.ResolveTitle("Details"));
			}

			return isInPageWithDetails;
		}

		private static MeterDetails[] MapMetersDetails(IEnumerable<IElement> elements)
		{
			return elements.Select((x, i) => new MeterDetails(x, i)).ToArray();
		}

		public class AccountAndMeterDetailsInfo
		{
			public AccountAndMeterDetailsInfo(IHtmlDocument document)
			{
				Document = document;
			}

			public IHtmlDocument Document { get; }

			public IHtmlElement Page => (IHtmlElement) Document.QuerySelector("[data-page='details']");

			public IHtmlElement AccountName =>
				(IHtmlElement) Page.QuerySelector("[data-testid='details-name-on-account']");

			public IHtmlElement AccountNumber =>
				(IHtmlElement) Page.QuerySelector("[data-testid='details-account-number']");

			public IHtmlElement SiteAddress =>
				(IHtmlElement) Page.QuerySelector("[data-testid='details-meter-address']");

			public IHtmlElement BillingAddress =>
				(IHtmlElement) Page.QuerySelector("[data-testid='details-billing-address']");

			public IHtmlCollection<IElement> MetersContainers =>
				Page.QuerySelectorAll("[data-testid^='details-meter-container-']");

			public MeterDetails[] MetersDetails => MapMetersDetails(MetersContainers);

			public IHtmlElement MeterConsumptionData =>
				(IHtmlElement) Page.QuerySelector("[data-testid='details-consumption-data-container']");

			public IHtmlAnchorElement MeterConsumptionDataDownloadLink =>
				(IHtmlAnchorElement) Page.QuerySelector("[data-testid='details-consumption-data-download']");
		}

		public class MeterDetails
		{
			public MeterDetails(IElement container, int index)
			{
				Container = container;
				Index = index;
			}

			public IElement Container { get; }

			public int Index { get; }

			public IHtmlElement MeterPointRefNumberLabel =>
				(IHtmlElement) Container.QuerySelector($"[data-testid='details-meter-{Index}-ref-number-label']");

			public IHtmlElement MeterPointRefNumber =>
				(IHtmlElement) Container.QuerySelector($"[data-testid='details-meter-{Index}-ref-number']");

			public IHtmlElement NetworksMeterNumber =>
				(IHtmlElement) Container.QuerySelector($"[data-testid='details-meter-{Index}-networks-meter-number']");

			public IHtmlElement Location =>
				(IHtmlElement) Container.QuerySelector($"[data-testid='details-meter-{Index}-location']");

			public IHtmlElement Description =>
				(IHtmlElement) Container.QuerySelector($"[data-testid='details-meter-{Index}-description']");

			public IElement CommsLevel =>
				Container.QuerySelector($"[data-testid='details-meter-{Index}-comms-level']");
		}
	}
}