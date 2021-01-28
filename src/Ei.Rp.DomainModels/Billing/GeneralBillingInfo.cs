using System;
using EI.RP.CoreServices.Cqrs.Queries;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.MappingValues;

namespace Ei.Rp.DomainModels.Billing
{
	public class GeneralBillingInfo : IQueryResult
	{
		public PaymentMethodType PaymentMethod { get; set; }

		public MeterReadingCategoryType MeterReadingType { get; set; }



		//TODO: MERGE BOTH FIELDS
		public DateTime? EqualizerNextBillDate { get; set; }
		public DateTime? EqualizerStartDate { get; set; }
		public DateTime NextBillDate { get; set; }

		public EuroMoney CurrentBalanceAmount { get; set; }

		public EuroMoney EqualizerAmount { get; set; }

		//TODO: not a really describing name, name refactor pending negative logic should be discouraged
		public bool IsUnEstimatedReading { get; set; }
		//TODO: not a really describing name, name refactor pending negative logic should be discouraged
		public bool HasNotPendingBillsToBeIssued { get; set; }
		public bool IsDeviceRefundAvailable { get; set; }

		public MonthlyBillingInfo MonthlyBilling { get; set; }=new MonthlyBillingInfo();

		public class MonthlyBillingInfo
		{
			public int? MonthlyBillingDayOfMonth { get; set; }

			public bool IsMonthlyBillingActive { get; set; }

			public bool CanSwitchToMonthlyBilling { get; set; }
		}

		

	}
}