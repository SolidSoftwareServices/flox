using EI.RP.UiFlows.Mvc.Components;
using EI.RP.UiFlows.Mvc.FlowTagHelpers;

namespace EI.RP.WebApp.Flows.AppFlows.SmartActivation.Components.SmartActivationNavigation
{
	public class ViewModel : FlowComponentViewModel
	{
		public bool IsAgentUser { get; set; }

		public ResidentialPortalFlowType SourceFlowType { get; set; }

		public FlowActionTagHelper.StartFlowLocation FlowLocation { get; set; }

		public bool ShowCancel { get; set; }
	}
}
