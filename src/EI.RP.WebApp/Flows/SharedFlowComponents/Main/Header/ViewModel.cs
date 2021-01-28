using Ei.Rp.DomainModels.Contracts;
using Ei.Rp.DomainModels.Contracts.Accounts;
using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.SharedFlowComponents.Main.Header
{
	public class ViewModel : FlowComponentViewModel
	{
		public string LayoutPathFragment { get; set; }
		public string Title { get; set; }
		public bool ShowAccountDetails { get; set; }
		public bool ShowAccountNumber { get; set; }
		public bool ShowAddress { get; set; }

		public string AccountNumber { get; set; }
		public string AccountDescription { get; set; }
		
	}
}
