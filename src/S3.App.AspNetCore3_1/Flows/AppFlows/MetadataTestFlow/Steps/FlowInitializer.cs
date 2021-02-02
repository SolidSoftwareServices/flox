using S3.App.AspNetCore3_1.Flows.AppFlows.GreenFlow.FlowDefinitions;
using S3.App.AspNetCore3_1.Flows.AppFlows.MetadataTestFlow.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Initialization;
using S3.UiFlows.Core.Flows.Screens;


namespace S3.App.AspNetCore3_1.Flows.AppFlows.MetadataTestFlow.Steps
{
	public class FlowInitializer : UiFlowInitializationStep<SampleAppFlowType>
	{
		public override SampleAppFlowType InitializerOfFlowType => SampleAppFlowType.MetadataTestFlow;

		public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(IScreenFlowConfigurator preStartCfg,
			UiFlowContextData contextData)
		{
			return preStartCfg.OnEventNavigatesTo(ScreenEvent.Start, MetadataTestFlowScreenScreenName.Step0Screen);
		}

		public override bool Authorize()
		{
			return true;
		}

		
	}
}