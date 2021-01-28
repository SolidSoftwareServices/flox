using System;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Sut;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Pages
{
	internal abstract class MyAccountEnergyServicesPage : SutPage<ResidentialPortalApp>
	{
		protected MyAccountEnergyServicesPage(ResidentialPortalApp app) : base(app)
		{

        }

        public MyProfileContextualMenu MyProfileMenu { get; private set; }

        protected override bool IsInPage()
        {
            var isInPage = Document.QuerySelector("[data-layout='energy-services']") != null;

            if (isInPage)
            {
                MyProfileMenu = new MyProfileContextualMenu(App, Document);
            }

            return isInPage;
        }

        public UserContactDetailsComponent ContactDetails => new UserContactDetailsComponent(this);

        public UserMarketingPreferenceComponent MarketingPreference => new UserMarketingPreferenceComponent(this);

        public ChangePasswordComponent ChangePassword => new ChangePasswordComponent(this);

        public class UserContactDetailsComponent
        {
            private MyAccountEnergyServicesPage ContainerPage { get; }

            public UserContactDetailsComponent(MyAccountEnergyServicesPage container)
            {
                ContainerPage = container;

            }
            public string ContactDetailsHeader => ContainerPage.Document
                .QuerySelector(
					"[data-testid='contact-details-heading']")
                .TextContent;
        }

        public class UserMarketingPreferenceComponent
        {
            private MyAccountEnergyServicesPage ContainerPage { get; }

            public UserMarketingPreferenceComponent(MyAccountEnergyServicesPage container)
            {
                ContainerPage = container;

            }

            public string MarketingPreferenceHeader => ContainerPage.Document
                .QuerySelector(
					"[data-testid='marketing-preference-heading']")
                .TextContent;

            public string MarketingPreferenceTopMessage => ContainerPage.Document
	            .QuerySelector(
		            "[data-testid='marketing-preference-top-message']")
	            .TextContent;

            public string MarketingPreferenceBottomMessage => ContainerPage.Document
	            .QuerySelector(
		            "[data-testid='marketing-preference-bottom-message']")
	            .TextContent;
        }

        public class ChangePasswordComponent
        {
            private MyAccountEnergyServicesPage ContainerPage { get; }

            public ChangePasswordComponent(MyAccountEnergyServicesPage container)
            {
                ContainerPage = container;

            }
            public string ChangePasswordHeader => ContainerPage.Document
                .QuerySelector(
                    "#changePasswordHeader")
                .TextContent;
        }

        public IHtmlElement AccountType =>
	        (IHtmlElement) Document.QuerySelector("#selectedAccount > div > span.accounts-list__utility > strong");


        public IHtmlAnchorElement LogoutButton => (IHtmlAnchorElement) Document.QuerySelector("[data-testid='main-navigation-log-out-link-desktop']");
        public async Task<ResidentialPortalApp> Logout()
        {
	        return (ResidentialPortalApp)await ClickOnElement(LogoutButton);
        }

		public IHtmlSpanElement AccountNumber =>
	        (IHtmlSpanElement) Document.QuerySelector("[data-testid='portal-header-account-number']");

        private async Task<ResidentialPortalApp> SelectMyAccountTab(string tabSelector)
        {
             await App.ToAccounts();
            var tabButton =(IHtmlElement)Document.QuerySelector(tabSelector);

            return (ResidentialPortalApp)await ClickOnElement(tabButton);
        }

        public async Task<ResidentialPortalApp> ToBillsAndPayments()
        {
            var tabSelector = "#billsAndPaymentsTab > a";
            return await SelectMyAccountTab(tabSelector);
        }



        public async Task<ResidentialPortalApp> ToContactUs()
        {
            var tabSelector = "#contactUsMenuItem > a";
            return await SelectMyAccountTab(tabSelector);
        }

        public async Task<ResidentialPortalApp> ToHelp()
        {
            var tabSelector = "#helpMenuItem > a";
            return await SelectMyAccountTab(tabSelector);
        }

        public async Task<ResidentialPortalApp> ToMyAccount()
        {
            var tabSelector = "#myAccountMenuItem > a";
            return await SelectMyAccountTab(tabSelector);
        }

        public async Task<ResidentialPortalApp> ToChangePassword()
        {
            await App.ToAccounts();
            return await MyProfileMenu.ClickOnChangePassword();
        }

        public async Task<ResidentialPortalApp> ToContactDetails()
        {
            await App.ToAccounts();
            return await MyProfileMenu.ClickOnContactDetails();
        }

        public async Task<ResidentialPortalApp> ToMarketingPreference()
        {
	        await App.ToAccounts();
	        var page = await MyProfileMenu.ClickOnContactDetails();
	        var altTabSelector = "[data-testid='sub-navigation-item-marketing']";

	        var tabButton = (IHtmlElement)page.CurrentPage.Document.QuerySelector(altTabSelector);

	        return (ResidentialPortalApp)await ClickOnElement(tabButton);
        }

		public class MyProfileContextualMenu
        {
            private readonly ResidentialPortalApp _app;
            private readonly IHtmlDocument _document;

            public MyProfileContextualMenu(ResidentialPortalApp app,IHtmlDocument document)
            {
                _app = app ?? throw new ArgumentNullException(nameof(app));
                _document = document ?? throw new ArgumentNullException(nameof(document));
            }

            public async Task<ResidentialPortalApp> ClickOnChangePassword()
            {
                return (ResidentialPortalApp)await _app.ClickOnElement(this.ChangePasswordAnchorElement);
            }

            public async Task<ResidentialPortalApp> ClickOnContactDetails()
            {
                return (ResidentialPortalApp)await _app.ClickOnElement(this.MyProfileAnchorElement);
            }


            public IHtmlAnchorElement ChangePasswordAnchorElement =>
                (IHtmlAnchorElement)_document.QuerySelector("[data-testid='main-navigation-change-password-link-desktop']");

            public IHtmlAnchorElement MyProfileAnchorElement =>
                (IHtmlAnchorElement)_document.QuerySelector("[data-testid='main-navigation-my-profile-link-desktop']");
        }
	}
}