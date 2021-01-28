using System;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EI.RP.UI.TestServices.Sut;
using EI.RP.WebApp.IntegrationTests.Infrastructure;

namespace EI.RP.WebApp.IntegrationTests.Sut.Parts.MyAccountEnergyServices.Pages.BillsAndPayments
{
    internal class EnergyServiceBillsAndPaymentsPage : MyAccountEnergyServicesPage
    {
        public EnergyServiceBillsAndPaymentsPage(ResidentialPortalApp app) : base(app)
        {
        }

        protected override bool IsInPage()
        {
           return base.IsInPage() && Document.QuerySelector("[data-page='energy-services-bills-and-payments']") != null;
        }

        public BillsPaymentsTableComponent BillsAndPayments => new BillsPaymentsTableComponent(this);

        public class BillsPaymentsTableComponent
        {
            private EnergyServiceBillsAndPaymentsPage ContainerPage { get; }

            public BillsPaymentsTableComponent(EnergyServiceBillsAndPaymentsPage container)
            {
                ContainerPage = container;

            }

            public IHtmlTableElement BillsPaymentsTable => 
                (IHtmlTableElement)ContainerPage.Document.QuerySelector("[data-testid='bills-and-payments-component']");

            public IHtmlParagraphElement DescriptionColumnValue =>
                (IHtmlParagraphElement)BillsPaymentsTable?.QuerySelector("tbody > tr:nth-child(1) [data-testid='bills-and-payments-component-description']");

            public IHtmlSpanElement DateColumnValue =>
                (IHtmlSpanElement)BillsPaymentsTable?.QuerySelector("tbody > tr:nth-child(1) [data-testid='bills-and-payments-component-date']");

            public IHtmlParagraphElement AmountColumnValue =>
                (IHtmlParagraphElement)BillsPaymentsTable?.QuerySelector("tbody > tr:nth-child(1) [data-testid='bills-and-payments-component-amount']");

        }

    }
}
