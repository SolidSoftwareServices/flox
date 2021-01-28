using System;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;

namespace Ei.Rp.DomainModels.Billing
{
	public class AccountBillingActivity : IQueryResult
	{
		public AccountBillingActivity(ActivitySource source)
		{
			if (source == ActivitySource.All)
			{
				throw new ArgumentOutOfRangeException(nameof(source));
			}
			Source = source;
		}

		public enum ActivitySource
		{
			All=0,
			Invoice=1,
			Payment
		}
		public ActivitySource Source { get; }
		
		public string AccountNumber { get; set; }
		public EuroMoney Amount { get; set; }
		public DateTime NextBillDate { get; set; }
        public string Description { get; set; }
		public DateTime OriginalDate { get; set; }
		public DateTime DueDate { get; set; }
		public DateTime? PrintDate { get; set; }
		public EuroMoney ReceivedAmount { get; set; }
		public EuroMoney CurrentBalanceAmount { get; set; }
        public EuroMoney DueAmount { get; set; }
        public PaymentMethodType PaymentMethod { get; set; }
		public MeterReadingCategoryType ReadingType { get; set; }
		public bool InvoiceFileAvailable { get; set; }
		public string ReferenceDocNumber { get; set; }

        
        public DateTime? EqualizerNextBillDate { get; set; }
        public bool LastBillIsBilled { get; set; }
    
		public EuroMoney EqualizerAmount { get; set; }

		public bool IsBill()
		{
			return Description != null && Description.Equals("bill", StringComparison.InvariantCultureIgnoreCase);
		}

		public bool IsBillReversal()
        {
	        return Description != null && Description.Equals("bill reversal", StringComparison.InvariantCultureIgnoreCase);
        }
        public bool IsPayment()
        {
            return Description != null && Description.Equals("payment", StringComparison.InvariantCultureIgnoreCase);
        }
        
         

        public bool IsUnEstimatedReading { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
        public EuroMoney RemainingAmount { get; set; }
        public string SubstituteDocument { get; set; }

        public override string ToString()
        {
	        return $"{nameof(Description)}: {Description}, {nameof(ReferenceDocNumber)}: {ReferenceDocNumber}, {nameof(OriginalDate)}: {OriginalDate},{nameof(DueDate)}: {DueDate}, {nameof(LastBillIsBilled)}: {LastBillIsBilled}, {nameof(InvoiceStatus)}: {InvoiceStatus}";
        }
	}
}