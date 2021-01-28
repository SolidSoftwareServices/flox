using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using Ei.Rp.DomainModels.MappingValues;
using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Components.AccountCard
{
	public class ViewModel : FlowComponentViewModel
	{
        public bool CanSubmitMeterReading { get; set; }
        public bool ShowBillingDetails { get; set; }

        public string AccountNumber { get;set; }
        public ClientAccountType AccountType { get;set; }

        public string AccountLocation {get;set; }

        public bool AccountIsOpen{get;set; }

	}
}
