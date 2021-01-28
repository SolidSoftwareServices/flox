using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Ei.Rp.DomainModels.Billing;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Pages.AccountOverview
{
	internal class EnergyServicesAccountOverviewPage : MyAccountEnergyServicesPage
	{
		public EnergyServicesAccountOverviewPage(ResidentialPortalApp app) : base(app)
		{ 
		}

        protected override bool IsInPage()
        {
            return base.IsInPage() && Document.QuerySelector("[data-page='energy-services-account-overview']") != null;
        }

        public BillingDetailsComponent LatestBill => new BillingDetailsComponent(this);
        public BillsAndPaymentsComponent BillsAndPayments => new BillsAndPaymentsComponent(this);
        public ContactUsComponent ContactUs => new ContactUsComponent(this);

        public class BillingDetailsComponent
		{
			private EnergyServicesAccountOverviewPage ContainerPage { get; }

			public BillingDetailsComponent(EnergyServicesAccountOverviewPage container)
			{
				ContainerPage = container;
			}

			public IHtmlParagraphElement CurrentBalanceAmount => (IHtmlParagraphElement)ContainerPage.Document
				.QuerySelector("[data-testid='energy-service-billing-details-component-amount']");

			public IHtmlSpanElement PaymentDate => (IHtmlSpanElement)ContainerPage.Document
				.QuerySelector("[data-testid='energy-service-billing-details-component-date']");
        }

        public class BillsAndPaymentsComponent
        {
            private EnergyServicesAccountOverviewPage ContainerPage { get; }

            public BillsAndPaymentsComponent(EnergyServicesAccountOverviewPage container)
            {
                ContainerPage = container;
            }

            public IHtmlTableElement BillsPaymentsTable => (IHtmlTableElement)ContainerPage.Document
                .QuerySelector("[data-testid='bills-and-payments-component']");

            public IHtmlParagraphElement DescriptionColumn => (IHtmlParagraphElement)BillsPaymentsTable
                .QuerySelector("tbody > tr:first-child [data-testid='bills-and-payments-component-description']");

            public IHtmlSpanElement DateColumn => (IHtmlSpanElement)ContainerPage.Document
                .QuerySelector("tbody > tr:first-child [data-testid='bills-and-payments-component-date']");

            public IHtmlParagraphElement AmountColumn => (IHtmlParagraphElement)ContainerPage.Document
                .QuerySelector("tbody > tr:first-child [data-testid='bills-and-payments-component-amount']");
        }

        public class ContactUsComponent
        {
            private EnergyServicesAccountOverviewPage ContainerPage { get; }

            public ContactUsComponent(EnergyServicesAccountOverviewPage container)
            {
                ContainerPage = container;
            }

            public IHtmlDivElement Component =>
                (IHtmlDivElement)ContainerPage.Document.QuerySelector("[data-testid='energy-services-account-overview-contact-us']");

            public IHtmlAnchorElement ContactUsLink =>
                (IHtmlAnchorElement)ContainerPage.Document.QuerySelector("[data-testid='energy-services-account-overview-contact-us-link']");

            public async Task<ResidentialPortalApp> ClickContactUsLink()
            {
                return (ResidentialPortalApp)await ContainerPage.ClickOnElement(ContactUsLink);
            }
        }
    }
}