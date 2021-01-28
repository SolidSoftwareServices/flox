using EI.RP.UiFlows.Mvc.FlowTagHelpers;

namespace EI.RP.WebApp.Flows.AppFlows.SmartActivation.Components.SmartActivationNavigation
{
	public class InputModel
	{
		public bool ShowCancel { get; set; } = true;

		public ResidentialPortalFlowType SourceFlowType { get; set; } = ResidentialPortalFlowType.Accounts;

		public FlowActionTagHelper.StartFlowLocation FlowLocation { get; set; } =
			FlowActionTagHelper.StartFlowLocation.ContainedInMe;
	}
}
