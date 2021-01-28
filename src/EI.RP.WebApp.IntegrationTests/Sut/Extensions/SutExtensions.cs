using System.Threading.Tasks;
using EI.RP.UI.TestServices.Sut;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Extensions
{
    internal static class SutExtensions
    {
        public static async Task<ResidentialPortalApp> ToPrivacyNotice(this SutPage<ResidentialPortalApp> sut)
        {
            return (ResidentialPortalApp)await sut.ClickOnElement(
                sut.Document.QuerySelector("[data-testid='privacy-notice-link']"));
        }

        public static async Task<ResidentialPortalApp> ToPrivacyNoticeViaComponent(this SutPage<ResidentialPortalApp> sut)
        {
            return (ResidentialPortalApp)await sut.ClickOnElement(
                sut.Document.QuerySelector("[data-testid='privacy-notice-message-component-link']"));
        }

        public static async Task<ResidentialPortalApp> ToHelpViaFooter(this SutPage<ResidentialPortalApp> sut)
        {
            return (ResidentialPortalApp)await sut.ClickOnElement(
                sut.Document.QuerySelector("[data-testid='footer-help-link']"));
        }

        public static async Task<ResidentialPortalApp> ToContactUsViaFooter(this SutPage<ResidentialPortalApp> sut)
        {
            return (ResidentialPortalApp)await sut.ClickOnElement(
                sut.Document.QuerySelector("[data-testid='footer-contact-us-link']"));
        }

        public static async Task<ResidentialPortalApp> ToTermsAndConditionsViaFooter(this SutPage<ResidentialPortalApp> sut)
        {
            return (ResidentialPortalApp)await sut.ClickOnElement(
                sut.Document.QuerySelector("[data-testid='footer-terms-and-conditions-link']"));
        }

		public static async Task<ResidentialPortalApp> ToCookiesPolicyViaCookiesNotice(this SutPage<ResidentialPortalApp> sut)
		{
			return (ResidentialPortalApp)await sut.ClickOnElement(
				sut.Document.QuerySelector("[data-testid='cookies-policy-link']"));
		}

		public static async Task<ResidentialPortalApp> ToDisclaimerViaFooter(this SutPage<ResidentialPortalApp> sut)
        {
            return (ResidentialPortalApp)await sut.ClickOnElement(
                sut.Document.QuerySelector("[data-testid='footer-disclaimer-link']"));
        }

        public static async Task<ResidentialPortalApp> ToPrivacyNoticeViaFooter(this SutPage<ResidentialPortalApp> sut)
        {
            return (ResidentialPortalApp)await sut.ClickOnElement(
                sut.Document.QuerySelector("[data-testid='footer-privacy-notice-link']"));
        }
    }
}