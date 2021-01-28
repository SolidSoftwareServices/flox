using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountElectricityAndGas.Pages.ContactUs
{
	internal class ContactUsPage : MyAccountElectricityAndGasPage
	{
		public ContactUsPage(ResidentialPortalApp app) : base(app)
		{
		}

		public IHtmlElement PageContainer =>
			(IHtmlElement) Document.QuerySelector("[data-page='contact-us']");

		public IHtmlElement AdditionalAccountComponent =>
			(IHtmlElement) PageContainer.QuerySelector("[data-testid='additional-account-component']");

		public IHtmlElement FaqBillsAndPaymentsComponent =>
			(IHtmlElement) PageContainer.QuerySelector("[data-testid='faq-bills-and-payments-component']");

		public IHtmlElement FaqMeterReadComponent =>
			(IHtmlElement) PageContainer.QuerySelector("[data-testid='faq-meter-read-component']");

		public IHtmlInputElement AccountNumberInput =>
			(IHtmlInputElement) PageContainer.QuerySelector("#accountNumber");

		public IHtmlInputElement MprnInput =>
			(IHtmlInputElement) PageContainer.QuerySelector("#meterNumber");

		public IHtmlSelectElement AccountDropDown => (IHtmlSelectElement) PageContainer
			.QuerySelector("#accountList");

		public IHtmlInputElement SelectedAccount => (IHtmlInputElement) PageContainer
			.QuerySelector("#selectedAccount");

		public IHtmlSelectElement QueryTypeDropDown => (IHtmlSelectElement) PageContainer
			.QuerySelector("#queryType");

		public IHtmlInputElement SubjectInput => (IHtmlInputElement) PageContainer
			.QuerySelector("#subject");

		public IHtmlTextAreaElement QueryInput => (IHtmlTextAreaElement) PageContainer
			.QuerySelector("#query");

		public IHtmlButtonElement SubmitQueryButton => PageContainer
			.QuerySelector("#btnSubmitQuery") as IHtmlButtonElement;

		public IHtmlButtonElement QueryTypeChangeButton => PageContainer
			.QuerySelector("[data-testid='step-event-query-changed']") as IHtmlButtonElement;

		public IHtmlInputElement SelectedQueryType =>
			(IHtmlInputElement) PageContainer.QuerySelector("#selectedQueryType");

		public IHtmlDivElement ErrorMessage =>
			PageContainer.QuerySelector("[data-testid='contact-us-error-message']") as IHtmlDivElement;

		public IHtmlAnchorElement MyDetailsProfileMenuItem => Document
		  .QuerySelector("[data-testid='main-navigation-my-profile-link-desktop']") as IHtmlAnchorElement;

		public IHtmlAnchorElement ChangePasswordProfileMenuItem => Document
			.QuerySelector("[data-testid='main-navigation-change-password-link-desktop']") as IHtmlAnchorElement;

		public IHtmlAnchorElement MarketingProfileMenuItem => Document
			.QuerySelector("[data-testid='main-navigation-marketing-link-desktop']") as IHtmlAnchorElement;

		public IHtmlAnchorElement LogoutProfileMenuItem => Document
			.QuerySelector("[data-testid='main-navigation-log-out-link-desktop']") as IHtmlAnchorElement;



		protected override bool IsInPage()
		{
			var isInPage = PageContainer != null;

			if (isInPage)
			{
				AssertTitle(App.ResolveTitle("Contact Us"));
			}

			return isInPage;
		}

		public IHtmlSelectElement SelectQueryType()
		{
			return Document.GetElementById("queryType") as IHtmlSelectElement;
		}

		public async Task<ResidentialPortalApp> SelectQueryType_AddAccount()
		{
			SelectedQueryType.Value = SelectQueryType().Options[1].Value;
			return (ResidentialPortalApp) await App.ClickOnElement(QueryTypeChangeButton);
		}

		public async Task<ResidentialPortalApp> ChangeQueryType(ContactQueryType queryType)
		{
			if (queryType == null) throw new ArgumentNullException(nameof(queryType));

			var queryOptions = SelectQueryType();
			var queryOptionToSelect =
				queryOptions.Options.FirstOrDefault(o => Uri.UnescapeDataString(o.Value) == queryType.ToString());

			if (queryOptionToSelect == null)
				throw new InvalidOperationException(
					$"QueryType: {queryType} doesn't exist is the dropdown");

			SelectedQueryType.Value = queryOptionToSelect.Value;
			return (ResidentialPortalApp) await App.ClickOnElement(QueryTypeChangeButton);
		}
	}
}