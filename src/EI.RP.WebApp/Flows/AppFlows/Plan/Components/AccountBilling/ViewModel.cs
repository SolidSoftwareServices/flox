using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Ei.Rp.DomainModels.MappingValues;
using Ei.Rp.Mvc.Core.ViewModels.Validations;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.AppFlows.Plan.Components.AccountBilling
{
	public class ViewModel : FlowComponentViewModel
	{
		public string AccountNumber { get; set; }
		public bool IsContractPending { get; set; }

		[DisplayName("Payment method")]
		public PaymentMethodType PaymentMethod { get; set; }

		public DirectDebitInfo DirectDebit { get; set; }
		

		public EqualiserInfo Equaliser { get; set; } = new EqualiserInfo();
		public PaperBillInfo PaperBill { get; set; } = new PaperBillInfo();
		public MonthlyBillingConfiguration MonthlyBilling { get; set; } = new MonthlyBillingConfiguration();
		public MeterDataInfo MeterData { get; set; } = new MeterDataInfo();

		public int? MonthlyBillingDayOfTheMonth { get; set; }
		
		public bool NoAccessToFeatures { get; set; }
		public bool MovedToStandardPlan { get; set; }
		public bool AgreeTermsAndConditions { get; set; }

		public class DirectDebitInfo
		{
			[DisplayName("Name on bank account")]
			public string NameInBankAccount { get; set; }

			[DisplayName("IBAN")]
			public string IBAN { get; set; }
		}

		public class EqualiserInfo
		{
			public bool IsVisible { get; set; }
			public bool IsContractPending { get; set; }
		}

		public class PaperBillInfo
		{
			public bool IsVisible { get; set; }
			public bool HasPaperBill { get; set; }
			public bool IsContractPending { get; set; }
			public bool IsSmartAccount { get; set; }

			public ScreenEvent SwitchOffPaperBillEvent { get; set; }
			public ScreenEvent SwitchOnPaperBillEvent { get; set; }

		}

		public class MonthlyBillingConfiguration
		{
			public ScreenEvent SwitchOffMonthlyBilling { get; set; }
			public ScreenEvent SwitchOnMonthlyBilling { get; set; }

			public bool IsMonthlyBillingActive { get; set; }

			public bool CanSwitchToMonthlyBilling { get; set; }

			public bool IsVisible { get; set; }
			public bool IsContractPending { get; set; }

			public bool CanEnableMonthlyPayments { get; set; }
		}

		public class MeterDataInfo
		{
			public bool IsVisible { get; set; }

			public bool IsContractPending { get; set; }

			public bool IsActive { get; set; }

			public bool IsChecked { get; set; }

			public bool HasValidationErrors { get; set; }

			public string PlanName { get; set; }

			public ScreenEvent SwitchOffMeterData { get; set; }
		}
	}
}