using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.WebApp.Flows.AppFlows.AccountDashboard.FlowDefinitions;

namespace EI.RP.WebApp.Flows.AppFlows.AccountDashboard.Steps
{
	public class ShowFlowGenericError : AccountDashboardScreen
	{
		public static class StepEvent
		{
			public static readonly ScreenEvent Next = new ScreenEvent(nameof(ShowFlowGenericError),nameof(Next));
		}
		public override ScreenName ScreenStep => AccountDashboardScreenStep.ShowFlowGenericError;
		public override string ViewPath => "FlowGenericError";
	}
}