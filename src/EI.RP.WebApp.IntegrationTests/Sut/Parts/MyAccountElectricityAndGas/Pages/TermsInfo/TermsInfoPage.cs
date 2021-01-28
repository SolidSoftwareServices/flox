using AngleSharp.Html.Dom;
using AngleSharp.Text;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.TermsInfo
{
	internal class TermsInfoPage : MyAccountElectricityAndGasPage
	{
		private readonly string[] _permittedPages =
		{
			"terms-and-conditions",
			"privacy-notice",
			"disclaimer"
		};

		public TermsInfoPage(ResidentialPortalApp app) : base(app)
		{
		}

		public string Page =>
			((IHtmlDivElement) Document.QuerySelector("div[data-page]"))?.GetAttribute("data-page");

		protected override bool IsInPage()
		{
			var isInPage = _permittedPages.Contains(Page);

			if (isInPage)
			{
				switch (Page)
				{
					case "terms-and-conditions":
						AssertTitle(App.ResolveTitle("Terms & Conditions"));
						break;
					case "privacy-notice":
						AssertTitle(App.ResolveTitle("Privacy Notice and Cookies Policy"));
						break;
					case "disclaimer":
						AssertTitle(App.ResolveTitle("Disclaimer"));
						break;
				}
			}

			return isInPage;
		}

		public bool IsTermsAndConditions()
		{
			return Page == "terms-and-conditions";
		}

		public bool IsPrivacyNotice()
		{
			return Page == "privacy-notice";
		}

		public bool IsDisclaimer()
		{
			return Page == "disclaimer";
		}
	}
}