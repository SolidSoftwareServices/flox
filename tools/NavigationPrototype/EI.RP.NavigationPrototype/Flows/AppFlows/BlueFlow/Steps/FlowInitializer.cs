using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using EI.RP.CoreServices.System;
using EI.RP.NavigationPrototype.Flows.AppFlows.BlueFlow.FlowDefinitions;
using EI.RP.NavigationPrototype.Flows.AppFlows.GreenFlow.Steps;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Initialization;
using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Metadata;


namespace EI.RP.NavigationPrototype.Flows.AppFlows.BlueFlow.Steps
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