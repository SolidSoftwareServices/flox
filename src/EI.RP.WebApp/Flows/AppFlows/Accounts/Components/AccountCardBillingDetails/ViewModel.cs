using System;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Components.AccountCardBillingDetails
{
	public class ViewModel : FlowComponentViewModel
	{
        public string AccountNumber { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
        public ClientAccountType AccountType { get; set; }

        public EuroMoney CurrentBalanceAmount { get; set; }
        public DateTime DueDate { get; set; }

        public EuroMoney EqualizerAmount { get; set; }
        public DateTime? EqualizerNextBillDate { get; set; }

        public EuroMoney BillAmount { get; set; }
        public DateTime LatestBillDate { get; set; }
        public DateTime NextBillDate { get; set; }

        public bool HasAccountCredit { get; set; }

        public bool IsDue { get; set; }
        public bool IsOverdue { get; set; }
        public bool IsEqualizer => PaymentMethod == PaymentMethodType.Equalizer;
        public bool IsAlternativePayer => PaymentMethod == PaymentMethodType.DirectDebitNotAvailable;

        public bool CanRequestRefund { get; set; }
        public bool CanEstimateCost { get; set; }
        public bool CanPayNow { get; set; }
        public bool AreBillsAllPaid { get; set; }
        public bool CanShowAdditionalMessage { get; set; }

        public string ReferenceDocNumber { get; set; }
        public bool InvoiceFileAvailable { get; set; }
        public bool HasIncludedLatestBillActionLink { get; set; }
        public bool CanShowLatestBillActionLink(bool isMobile = false) => InvoiceFileAvailable && (!HasIncludedLatestBillActionLink || isMobile);

        public bool CanShowCostToDate { get; set; }
        public EuroMoney CostToDateAmount { get; set; } = EuroMoney.Zero;
        public DateTime CostToDateSince { get; set; } = new DateTime();
    }
}
