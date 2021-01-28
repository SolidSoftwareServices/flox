using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Components.AccountCard
{
	public class InputModel
    {
        public bool CanSubmitMeterReading { get; set; }
        public bool ShowBillingDetails { get; set; }
		public string AccountNumber { get; set; }
    }
}
