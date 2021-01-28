using S3.App.AspNetCore3_1.Flows.AppFlows.ContainersFlow4.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.Infrastructure.DataSources;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Initialization;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.AspNetCore3_1.Flows.AppFlows.ContainersFlow4.Steps
{
	public class FlowInitializer : UiFlowInitializationStep<SampleAppFlowType>
	{

		public override SampleAppFlowType InitializerOfFlowType => SampleAppFlowType.ContainersFlow4;

		public override bool Authorize()
		{
			return true;
		}


		public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(IScreenFlowConfigurator preStartCfg,
			UiFlowContextData contextData)
		{
			return preStartCfg.OnEventNavigatesTo(ScreenEvent.Start, ContainersFlow4ScreenName.Number1ContainerScreen);
		}
	}
}