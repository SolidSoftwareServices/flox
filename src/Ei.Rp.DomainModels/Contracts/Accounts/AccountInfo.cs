using System;
using System.Collections.Generic;
using System.Linq;
using EI.RP.CoreServices.System;
using Ei.Rp.DomainModels.Banking;
using Ei.Rp.DomainModels.ComplexTypes.PointReferenceNumbers;
using Ei.Rp.DomainModels.MappingValues;

namespace Ei.Rp.DomainModels.Contracts.Accounts
{
	public partial class AccountInfo : AccountBase
    {
        public bool IsPAYGCustomer { get; set; }
      
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
     
        public string ContractId { get; set; }

        public string PremiseConnectionObjectId { get; set; }
		public PaymentMethodType PaymentMethod { get; set; }

        public DateTime? ContractStartDate { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public PaperBillChoice PaperBillChoice { get; set; }

		public string BundleReference { get; set; }
        public IEnumerable<BankAccountInfo> BankAccounts { get; set; }
        public string BillToAccountAddressId { get; set; }
        
		public bool IsLossInProgress { get; set; }
		


        public bool IsSmart() => SmartPeriods.Any(x => x.Contains(DateTime.Today));
 
		public bool SwitchToSmartPlanDismissed { get; set; }

		/// <summary>
		/// is a collection of periods where the HH data exist for the account.
		/// </summary>
		public IEnumerable<DateTimeRange> SmartPeriods { get; set; } = new DateTimeRange[0];

		/// <summary>
		/// is a collection of periods where the HH data exist for the account.
		/// </summary>
		public IEnumerable<DateTimeRange> NonSmartPeriods { get; set; } = new DateTimeRange[0];

		public string PlanName { get; set; }

		public decimal DiscountAppliedPercentage{ get; set; }

		public BusinessAgreement BusinessAgreement { get; set; }

		public BankAccountInfo IncomingBankAccount { get; set; }

		public SmartActivationStatus SmartActivationStatus { get; set; }

		public bool HasStaffDiscountApplied { get; set; }

		public bool HasQuotationsInProgress { get; set; }

		public bool CanSwitchToStandardPlan { get; set; }
		
		public bool CanSubmitMeterReading { get; set; }
	}
}