using S3.App.Flows.AppFlows.StartFailure.FlowDefinitions;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.Flows.AppFlows.StartFailure.Steps
{
	public class FailureScreen : StartFailureFlowScreen
	{
		
		
		public override ScreenName ScreenStep =>  StartFailureFlowScreenName.FailureScreen;
		public override string ViewPath => "ErrorView";

		
	}
}
