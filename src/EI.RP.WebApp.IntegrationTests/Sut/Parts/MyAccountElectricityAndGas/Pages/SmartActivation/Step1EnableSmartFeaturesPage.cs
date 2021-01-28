using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Dynamitey;
using EI.RP.UI.TestServices.Sut;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation.Components;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.SmartActivation
{
	internal class Step1EnableSmartFeaturesPage : SmartActivationPage
	{
		public Step1EnableSmartFeaturesPage(ResidentialPortalApp app) : base(app)
		{
		}

		protected override bool IsInPage()
		{
			var isInPage = base.IsInPage() && Page != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("1. Smart Features | Smart sign up"));
			}

			return isInPage;
		}

		public IHtmlElement Page => (IHtmlElement)Document.QuerySelector("[data-page='step1-enable-smart-features']");
		public IHtmlInputElement InformationCollectionAuthorizedCheckbox  => (IHtmlInputElement)Document.QuerySelector("#informationCollectionAuthorized");
		public IHtmlSpanElement InformationCollectionAuthorizedValidation => (IHtmlSpanElement)Document.QuerySelector("[data-testid='information-collection-authorized-validation']");
		public IHtmlElement EnableSmartServicesButton => (IHtmlElement)Document.QuerySelector("#enableSmartServicesButton");
		public IHtmlElement NoThanksSmartActivationLink => (IHtmlElement)Document.QuerySelector("[data-testid='no-thanks-smart-activation-yes']");
		public IHtmlAnchorElement MoreAboutSmartMetersLink => (IHtmlAnchorElement)Document.QuerySelector("[data-testid='more-about-smart-meters-link']");
		public IHtmlAnchorElement SmartActivationPrivacyNoticeLink => (IHtmlAnchorElement) Document.QuerySelector("[data-testid='smart-activation-privacy-notice-link']");

		

		public async Task<ISutApp> NextPage(bool? authorize=null)
		{
			if(authorize.HasValue)
				InformationCollectionAuthorizedCheckbox.IsChecked = authorize.Value;
			return await ClickOnElement(EnableSmartServicesButton);
		}
	}
}