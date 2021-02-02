using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using S3.CoreServices.System;
using S3.App.AspNetCore3_1.Flows.AppFlows.BlueFlow.FlowDefinitions;
using S3.App.AspNetCore3_1.Flows.AppFlows.GreenFlow.Steps;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Initialization;
using S3.UiFlows.Core.Flows.Initialization.Models;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Metadata;


namespace S3.App.AspNetCore3_1.Flows.AppFlows.BlueFlow.Steps
{
	public class FlowInitializer : UiFlowInitializationStep<SampleAppFlowType, FlowInitializer.StartScreenModel>
	{

		public override SampleAppFlowType InitializerOfFlowType => SampleAppFlowType.BlueFlow;
		

		public override IScreenFlowConfigurator OnDefiningAdditionalInitialStateTransitions(IScreenFlowConfigurator preStartCfg,
			UiFlowContextData contextData)
		{
			
			return preStartCfg
				.OnEventNavigatesTo(ScreenEvent.ErrorOccurred, BlueFlowScreenName.ErrorScreen)
				.OnEventNavigatesTo(ScreenEvent.Start, BlueFlowScreenName.Step0Screen);
		}

		protected override Task<StartScreenModel> OnBuildStartData(UiFlowContextData newContext, StartScreenModel preloadedInputData)
		{
			if (nameof(FlowInitializer).Equals(preloadedInputData.FailOnStep, StringComparison.InvariantCultureIgnoreCase)
			    && preloadedInputData.FailOnEvent==ScreenLifecycleStage.FlowInitialization)
			{
				throw new Exception($"Triggering failure on {GetType().Name}");
			}
			return base.OnBuildStartData(newContext, preloadedInputData);
		}

		public override bool Authorize()
		{
			return true;
		}



		public class StartScreenModel : InitialFlowScreenModel, IBlueInput
		{
			public string GreenFlowData { get; set; }
			public string FailOnStep { get; set; }
			public ScreenLifecycleStage? FailOnEvent { get; set; }
		}
	}
}