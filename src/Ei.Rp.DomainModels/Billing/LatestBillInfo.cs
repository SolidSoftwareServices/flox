using System;
using EI.RP.CoreServices.System;

namespace Ei.Rp.DomainModels.Billing
{
	public  class LatestBillInfo : GeneralBillingInfo
	{
		
		public EuroMoney Amount { get; set; }
		public bool InvoiceFileAvailable { get; set; }

		public string ReferenceDocNumber { get; set; }

		public DateTime LatestBillDate { get; set; }
		public DateTime DueDate { get; set; }
		public string AccountNumber { get; set; }
		public bool HasAccountCredit { get; set; }
		public bool CostCanBeEstimated { get; set; }

        public string AccountDescription { get; set; }

        public bool AccountIsOpen { get; set; }
		public bool CanRequestRefund { get; set; }
		public bool CanAddGasAccount { get; set; }
		
        public bool CanShowCostToDate { get; set; }
        public EuroMoney CostToDateAmount { get; set; }
        public DateTime CostToDateSince { get; set; }

        public static LatestBillInfo From(GeneralBillingInfo generalBilling)
		{

			return new LatestBillInfo
			{
				CurrentBalanceAmount = generalBilling.CurrentBalanceAmount,
				EqualizerAmount = generalBilling.EqualizerAmount,
				EqualizerNextBillDate = generalBilling.EqualizerNextBillDate,
				IsUnEstimatedReading = generalBilling.IsUnEstimatedReading,
				HasNotPendingBillsToBeIssued = generalBilling.HasNotPendingBillsToBeIssued,
				MeterReadingType = generalBilling.MeterReadingType,
				NextBillDate = generalBilling.NextBillDate,
				PaymentMethod = generalBilling.PaymentMethod
            };
		}
	}
}