using AngleSharp.Html.Dom;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.UserContactDetails
{
	internal class MarketingPreferencePage : MyAccountElectricityAndGasPage
	{
		public MarketingPreferencePage(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Marketing"));
			}

			return isInPage;
		}

		public IHtmlElement Page => Document.QuerySelector("[data-pageid='marketing-preferences']") as IHtmlElement;

		public IHtmlInputElement SmsPreference => Page.QuerySelector("#chkSms") as IHtmlInputElement;

		public IHtmlInputElement DoorToDoorPreference => Page.QuerySelector("#chkDoorToDoor") as IHtmlInputElement;

		public IHtmlInputElement LandLinePreference => Page.QuerySelector("#chkLandLine") as IHtmlInputElement;

		public IHtmlInputElement MobilePreference => Page.QuerySelector("#chkMobile") as IHtmlInputElement;
		
		public IHtmlInputElement EmailPreference => Page.QuerySelector("#chkEmail") as IHtmlInputElement;

		public IHtmlInputElement PostPreference => Page.QuerySelector("#chkPost") as IHtmlInputElement;

		public IHtmlElement SuccessMessage => Page.QuerySelector("#successMessage") as IHtmlElement;

		public IHtmlButtonElement SaveChangesButton => Page.QuerySelector("#btnSaveContactDetails") as IHtmlButtonElement;

	}
}
