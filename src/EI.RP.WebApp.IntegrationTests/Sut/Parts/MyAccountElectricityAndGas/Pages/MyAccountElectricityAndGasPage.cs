using System;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Sut;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.AccountsPaymentConfiguration;
using EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.ProductAndServices;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages
{
	internal abstract class MyAccountElectricityAndGasPage : SutPage<ResidentialPortalApp>
	{
		protected MyAccountElectricityAndGasPage(ResidentialPortalApp app) : base(app)
		{

        }

		protected override bool IsInPage()
        {
            var isInPage = Document.QuerySelector("[data-layout='electricity-and-gas']") != null;

            return isInPage;
        }

        public IHtmlAnchorElement LogoutButton => (IHtmlAnchorElement) Document.QuerySelector("[data-testid='main-navigation-log-out-link-desktop']");

        public async Task<ResidentialPortalApp> Logout()
        {
	        return (ResidentialPortalApp)await ClickOnElement(LogoutButton);
        }

		public IHtmlSpanElement AccountNumber =>
	        (IHtmlSpanElement) Document.QuerySelector("[data-testid='portal-header-account-number']");

		private async Task<ResidentialPortalApp> ClickOn(string selector, bool selectFirstAccount = true, string accountNumber = null)
        {
	        var app = await App.ToAccounts();

	        if (selectFirstAccount)
	        {
		        app = await app.ToFirstAccount();
	        }
            else if (!string.IsNullOrWhiteSpace(accountNumber))
	        {
		        app = await app.ToAccount(accountNumber);
	        }

	        var el = (IHtmlElement)app.CurrentPage.Document.QuerySelector(selector);

	        return (ResidentialPortalApp)await ClickOnElement(el);
        }

        public async Task<ResidentialPortalApp> ToUsage()
        {
	        return await ClickOn("[data-testid='nav-usage-link']");
        }

        public async Task<ResidentialPortalApp> ToBillingAndPayments()
        {
	        return await ClickOn("[data-testid='nav-payments-link']");
        }

        public async Task<ResidentialPortalApp> ToMeterReading()
        {
	        return await ClickOn("[data-testid='nav-meter-reading-link']");
        }

        public async Task<ResidentialPortalApp> ToProductAndServices()
        {
	        return await ClickOn("[data-testid='main-navigation-products-and-services-link']", false);
        }

        public async Task<ResidentialPortalApp> ToPlanPage()
        {
	        return await ClickOn("[data-testid='nav-plan-link']");
        }

        public async Task<ResidentialPortalApp> ToMovingHome(string accountNumber = null)
        {
	        var selectFirstAccount = string.IsNullOrWhiteSpace(accountNumber);

	        return await ClickOn("[data-testid='nav-moving-house-link']", selectFirstAccount, accountNumber);
        }

		public async Task<ResidentialPortalApp> ToAccountAndMeterDetails()
        {
	        return await ClickOn("[data-testid='nav-details-link']");
        }

        public async Task<ResidentialPortalApp> ToMakeAPayment()
        {
	        var app = (await ToBillingAndPayments()).CurrentPageAs<ShowPaymentsHistoryPage>();
	        var el = (IHtmlElement)app.Document.QuerySelector("[data-testid='payments-history-pay-now-button']");

	        return (ResidentialPortalApp)await ClickOnElement(el);
        }

        public async Task<ResidentialPortalApp> ToContactUs()
        {
	        return await ClickOn("[data-testid='main-navigation-contact-us-link']", false);
        }

        public async Task<ResidentialPortalApp> ToDisclaimer()
        {
	        return await ClickOn("[data-testid='footer-disclaimer-link']", false);
        }

        public async Task<ResidentialPortalApp> ToTermsAndCondition()
        {
	        return await ClickOn("[data-testid='footer-terms-and-conditions-link']", false);
        }

        public async Task<ResidentialPortalApp> ToHelp()
        {
	        return await ClickOn("[data-testid='main-navigation-help-link']", false);
        }

        public async Task<ResidentialPortalApp> ToPrivacy()
        {
	        return await ClickOn("[data-testid='footer-privacy-notice-link']", false);
        }

        public async Task<ResidentialPortalApp> ToChangePassword()
        {
	        return await ClickOn("[data-testid='main-navigation-change-password-link-desktop']", false);
        }

        public async Task<ResidentialPortalApp> ToContactDetails(string accountNumber = null)
        {
            return await ClickOn("[data-testid='main-navigation-my-profile-link-desktop']", true, accountNumber);
        }

        public async Task<ResidentialPortalApp> ToMarketingPreference(string accountNumber = null)
        {
	        return await ClickOn("[data-testid='main-navigation-marketing-link-desktop']", true, accountNumber);
		}

        public async Task<ResidentialPortalApp> ToRequestRefund()
        {
	        return await ClickOn($"[data-testid^='account-card-submit-meter-reading-']", false);
        }

        public async Task<ResidentialPortalApp> ToCompetitionEntry()
        {
	        var app = (await ToProductAndServices()).CurrentPageAs<ProductAndServicesPage>();
	        var el = (IHtmlElement)app.Document.QuerySelector("[data-testid='sub-navigation-item-competitions']");

	        return (ResidentialPortalApp)await ClickOnElement(el);
        }

        public async Task<ResidentialPortalApp> ToPromotionEntry()
        {
	        var app = (await ToProductAndServices()).CurrentPageAs<ProductAndServicesPage>();
	        var el = (IHtmlElement)app.Document.QuerySelector("[data-testid='sub-navigation-item-promotions']");

	        return (ResidentialPortalApp)await ClickOnElement(el);
        }
	}
}