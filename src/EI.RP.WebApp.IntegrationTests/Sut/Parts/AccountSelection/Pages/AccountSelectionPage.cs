using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Sut;
using EI.RP.UiFlows.Mvc;
using EI.RP.WebApp.IntegrationTests.Infrastructure;
using NUnit.Framework;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.AccountSelection.Pages
{
	internal class AccountSelectionPage : SutPage<ResidentialPortalApp>
	{
		public AccountSelectionPage(ResidentialPortalApp app) : base(app)
		{
			OpenAccountLatestBills = new Lazy<LatestBillComponent[]>(() => Document
				.QuerySelectorAll("div[name='openAccountItem']")
				.Select(x => new LatestBillComponent(App, (IHtmlElement) x)).ToArray());
		}

        protected override bool IsInPage()
        {
	        var pageIsOpenAccounts = Document.Body.QuerySelector("[data-page='my-accounts-open']") != null;
	        var pageIsClosedAccounts = Document.Body.QuerySelector("[data-page='my-accounts-closed']") != null;

	        var isInPage = pageIsOpenAccounts || pageIsClosedAccounts;

	        if (isInPage)
	        {
		        AssertTitle(App.ResolveTitle(pageIsOpenAccounts ? "My Accounts" : "Closed Accounts"));
	        }

            return isInPage;
        }

		public Lazy<LatestBillComponent[]> OpenAccountLatestBills { get; }


		public IHtmlAnchorElement ClosedAccountLink =>
			(IHtmlAnchorElement) Document.QuerySelector("[data-testid='view-closed-accounts-link']");

		public IHtmlAnchorElement OpenAccountLink =>
			(IHtmlAnchorElement) Document.QuerySelector("[data-testid='view-open-accounts-link']");

		public IHtmlAnchorElement LogoUrl => Document.QuerySelector("[data-testid='home_url']") as IHtmlAnchorElement;

		public IHtmlElement Notifications => Document.QuerySelector("[data-testid='notifications']") as IHtmlElement;
		public IHtmlElement SmartActivationNotification => Notifications.QuerySelector("[data-testid='smart-activation-notification']") as IHtmlElement;
        public IHtmlElement SmartActivationNotificationClose => Notifications.QuerySelector("[id='smartActivationBannerClose']") as IHtmlElement;
        public IHtmlElement SmartActivationNotificationMprn => Notifications.QuerySelector("[data-testid='smart-activation-mprn']") as IHtmlElement;
        public IHtmlElement SmartActivationNotificationJourneyLink => Notifications.QuerySelector("[data-testid='smart-activation-journey']") as IHtmlElement;

        public IHtmlElement Banners => Document.QuerySelector("[data-testid='banners']") as IHtmlElement;
		public IHtmlElement PromotionBannerClose => Banners.QuerySelector("[id='promoBoxClose']") as IHtmlElement;
		public IHtmlElement PromotionEntry => Banners.QuerySelector("[id='promotionEntry']") as IHtmlElement;
        public IHtmlElement CompetitionBannerClose => Banners.QuerySelector("[data-testid='competitionBoxClose']") as IHtmlElement;
		public IHtmlElement CompetitionEntry => Banners.QuerySelector("[id='competitionEntry']") as IHtmlElement;

        public IHtmlButtonElement SelectAccountBtn(string accountNumber)
        {
            return (IHtmlButtonElement) Document.QuerySelector(
                $"[data-testid='account-card-view-this-account-{accountNumber}']");
        }

        public IHtmlButtonElement SubmitMeterReadingBtn(string accountNumber)
        {
            var btn = (IHtmlButtonElement) Document.QuerySelector(
                $"[data-testid='account-card-submit-meter-reading-{accountNumber}']");

            return btn;
        }

        private string ResolveLinkSelector(string itemId)
        {
	        return $"#Accounts-{itemId} > input[type=hidden][name={SharedSymbol.FlowEventFormFieldName}]";

        }
        public async Task<ResidentialPortalApp> ToHelp()
        {
            var helpLink = (IHtmlInputElement)Document.QuerySelector(ResolveLinkSelector("_help"));
            helpLink.Value = Flows.AppFlows.Accounts.Steps.AccountSelection.StepEvent.ToHelp;

            return (ResidentialPortalApp)await ClickOnElement(helpLink);
        }
        public async Task<ResidentialPortalApp> ToContactUs()
        {
            var helpLink = (IHtmlInputElement)Document.QuerySelector(ResolveLinkSelector("_contact_us"));
            helpLink.Value = Flows.AppFlows.Accounts.Steps.AccountSelection.StepEvent.ToContactUs;

            return (ResidentialPortalApp)await ClickOnElement(helpLink); ;
        }
        public async Task<ResidentialPortalApp> ToDisclaimer()
        {
            var helpLink = (IHtmlInputElement)Document.QuerySelector(ResolveLinkSelector("_disclaimer"));
            helpLink.Value = Flows.AppFlows.Accounts.Steps.AccountSelection.StepEvent.ToDisclaimer;

            return (ResidentialPortalApp)await ClickOnElement(helpLink); ;
        }
        public async Task<ResidentialPortalApp> ToPrivacy()
        {
            var helpLink = (IHtmlInputElement)Document.QuerySelector(ResolveLinkSelector("_privacy"));
            helpLink.Value = Flows.AppFlows.Accounts.Steps.AccountSelection.StepEvent.ToPrivacy;

            return (ResidentialPortalApp)await ClickOnElement(helpLink); ;
        }
        public async Task<ResidentialPortalApp> ToTermsAndConditions()
        {
            var helpLink = (IHtmlInputElement)Document.QuerySelector(ResolveLinkSelector("_terms_conditions"));
            helpLink.Value = Flows.AppFlows.Accounts.Steps.AccountSelection.StepEvent.ToTermsAndConditions;

            return (ResidentialPortalApp)await ClickOnElement(helpLink); ;
        }
        public IEnumerable<string> ReadAccountNumbers()
		{
			var accountNumberPs = Document
                .QuerySelectorAll("[data-testid='account-card-account-number']");

			return accountNumberPs.Select(x => x.TextContent).ToArray();
		}

        public async Task<ResidentialPortalApp> ToSmartActivation(string accountNumber = null)
        {
			return (ResidentialPortalApp) (await ClickOnElement( Document.QuerySelector("[data-testid='smart-activation-journey']")));
        }

		public async Task<ResidentialPortalApp> SelectFirstAccount()
        {
            return (ResidentialPortalApp) await ClickOnElement(
                (IHtmlButtonElement) Document.QuerySelector(
                    $"[data-testid^='account-card-view-this-account-']"));
        }

		public async Task<ResidentialPortalApp> SelectAccount(string accountNumber)
		{
            return (ResidentialPortalApp) await ClickOnElement(SelectAccountBtn(accountNumber));
		}

        public async Task<ResidentialPortalApp> ClickSubmitMeterReading(int index)
        {
            return (ResidentialPortalApp) await ClickOnElement(
                (IHtmlButtonElement) Document.QuerySelectorAll(
                    $"[data-testid^='account-card-submit-meter-reading-']")[index]);
        }

        public async Task<ResidentialPortalApp> ClickEstimateButton()
        {
            return (ResidentialPortalApp) await ClickOnElement(
                Document.QuerySelector("[data-testid='account-card-estimate-cost']"));
        }

        public async Task<ResidentialPortalApp> ClickMakeAPayment(int index)
        {
            return (ResidentialPortalApp) await ClickOnElement(
                Document.QuerySelectorAll("[data-testid='account-card-pay-now']")[index]);
        }

        public async Task<ResidentialPortalApp> Logout()
		{
			return (ResidentialPortalApp) await ClickOnElement(Document.QuerySelector("[data-testid='main-navigation-log-out-link-desktop']"));
		}

        public async Task<ResidentialPortalApp> ToChangePassword()
        {
            return (ResidentialPortalApp) await ClickOnElement(
                Document.QuerySelector("[data-testid='main-navigation-change-password-link-desktop']"));
        }

        public async Task<ResidentialPortalApp> ToRequestRefund(int index)
        {
	        return (ResidentialPortalApp) await ClickOnElement(
		        (IHtmlButtonElement) Document.QuerySelectorAll(
			        $"[data-testid^='account-card-submit-refund-request-']")[index]);
        }

        public async Task<ResidentialPortalApp> MyAccountTabClick()
        {
            return (ResidentialPortalApp)await ClickOnElement(MyAccounts);
        }

        public IHtmlAnchorElement MyAccounts => 
            (IHtmlAnchorElement)Document.QuerySelector("[data-testid='main-navigation-my-accounts-link']");

        public class LatestBillComponent
		{
			private readonly ResidentialPortalApp _app;

			public LatestBillComponent(ResidentialPortalApp app, IHtmlElement element)
			{
				_app = app;
				Element = element;
			}

			private IHtmlElement Element { get; }

			public IHtmlHeadingElement LatestBillHeader => (IHtmlHeadingElement) Element
				.QuerySelector(
					"#latestBillHeader");

			public IHtmlListItemElement BillAmount => (IHtmlListItemElement) Element
				.QuerySelector(
					"#ao_bill_amount_value");


			public IHtmlListItemElement ReadingType => (IHtmlListItemElement) Element
				.QuerySelector(
					"#ao_reading_type_value");

			public IHtmlSpanElement CurrentBalanceAmount => (IHtmlSpanElement) Element
				.QuerySelector(
					"#dueAmount > span");

			public string DownLoadPdf => Element.QuerySelector("#ao_latest_bill_pdf_btn").TextContent;

			public IHtmlSpanElement AccountNumber =>
				(IHtmlSpanElement) Element.QuerySelector(
					"#account_details_header > p:nth-child(1) > span.utility > span");
		}
	}
}