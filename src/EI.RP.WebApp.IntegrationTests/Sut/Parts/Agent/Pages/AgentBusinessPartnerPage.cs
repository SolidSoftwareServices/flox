using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Sut;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.Agent.Pages
{
    internal class AgentBusinessPartnerPage : SutPage<ResidentialPortalApp>
    {
        public AgentBusinessPartnerPage(ResidentialPortalApp app) : base(app)
        {
        }

        protected override bool IsInPage()
        {
            return BusinessPartnerPage != null;
        }

        public IHtmlElement BusinessPartnerPage => (IHtmlElement) Document.QuerySelector("[data-page='business-partners-search']");
        public IHtmlAnchorElement LogoUrl =>  Document.QuerySelector("[data-testid='home_url']") as IHtmlAnchorElement;
        public IElement MyProfileButton => Document.QuerySelector("[data-testid='btn-my-profile']");

        public IHtmlAnchorElement ChangePasswordMenuItemLink =>  (IHtmlAnchorElement)Document.QuerySelector("[data-testid='main-navigation-change-password-link-desktop']");
        public IHtmlAnchorElement LogoutMenuItemLink =>  (IHtmlAnchorElement)Document.QuerySelector("[data-testid='main-navigation-log-out-link-desktop']");
        

        public IHtmlInputElement UserNameInput => BusinessPartnerPage.QuerySelector("#FirstName") as IHtmlInputElement;
        public IHtmlInputElement CityInput => BusinessPartnerPage.QuerySelector("#City") as IHtmlInputElement;
        public IHtmlInputElement MaximumRecordsInput => BusinessPartnerPage.QuerySelector("#MaxNo") as IHtmlInputElement;
        public IHtmlElement ErrorMessage => BusinessPartnerPage.QuerySelector("[data-testid='error-message']") as IHtmlElement;
        public IHtmlElement Pagination => BusinessPartnerPage.QuerySelector("[data-testid='search-result-pages']") as IHtmlElement;

		public IHtmlButtonElement FindBusinessPartnersButton => BusinessPartnerPage.QuerySelector("#btnSubmit") as IHtmlButtonElement;

		public IHtmlButtonElement ViewBusinessPartnersButton => BusinessPartnerPage.QuerySelector("[data-testid='btn-view-business-partner']") as IHtmlButtonElement;
    }
}