using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.AppFlows.BusinessPartnersSearch.Components.SearchResults
{
	public class ViewModel : FlowPageableComponentViewModel<ViewModel.Row>
	{
		public bool ShowDeRegistrationColumn { get; set; }

		public bool IsSingleUserBusinessPartner { get; set; }

		public class Row
		{
			public string PartnerNumber { get; set; }
			public string UserName { get; set; }
			public string Description { get; set; }
		}
	}
}