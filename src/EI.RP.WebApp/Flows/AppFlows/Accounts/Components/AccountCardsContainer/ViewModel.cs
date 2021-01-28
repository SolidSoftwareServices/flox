using EI.RP.UiFlows.Mvc.Components;

namespace EI.RP.WebApp.Flows.AppFlows.Accounts.Components.AccountCardsContainer
{
	public class ViewModel : FlowPageableComponentViewModel<ViewModel.Row>
	{
		public bool CanSubmitMeterReading { get; set; }
        public string ContainerId { get; set; }
		public string PaginationId { get; set; }
		public string WhenChangingPageFocusOn { get; set; }

		public class Row
		{
			public string AccountNumber { get; set; }

			public string Partner{ get; set; }

			public bool IsOpen{ get; set; }
		}

		
    }
}
