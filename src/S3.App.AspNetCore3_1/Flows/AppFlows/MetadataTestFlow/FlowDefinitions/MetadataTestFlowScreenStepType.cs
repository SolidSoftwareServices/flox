using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.AspNetCore3_1.Flows.AppFlows.MetadataTestFlow.FlowDefinitions
{


	public static class MetadataTestFlowScreenScreenName
	{
		public static readonly ScreenName Step0Screen = new ScreenName($"MetadataTestFlow_{nameof(Step0Screen)}");
	}
}