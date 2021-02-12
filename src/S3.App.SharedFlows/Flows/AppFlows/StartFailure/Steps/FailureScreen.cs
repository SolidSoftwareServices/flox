using S3.App.Flows.AppFlows.StartFailure.FlowDefinitions;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.Flows.AppFlows.StartFailure.Steps
{
	public class FailureScreen : UiFlowScreen
	{
		
		
		public override ScreenName ScreenNameId =>  StartFailureFlowScreenName.FailureScreen;
		public override string ViewPath => "ErrorView";

		
	}
}
