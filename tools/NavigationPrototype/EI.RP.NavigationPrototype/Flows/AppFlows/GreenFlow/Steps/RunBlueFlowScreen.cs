using System.Collections.Generic;
using System.Threading.Tasks;
using EI.RP.NavigationPrototype.Flows.AppFlows.GreenFlow.FlowDefinitions;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Flows.Screens.Models.Interop;

using Newtonsoft.Json;

namespace EI.RP.NavigationPrototype.Flows.AppFlows.GreenFlow.Steps
{
	public class RunBlueFlowScreen : GreenFlowScreen
	{
		public static class StepEvent
		{
			public static readonly ScreenEvent BlueFlowCompleted = new ScreenEvent(nameof(RunBlueFlowScreen), nameof(BlueFlowCompleted));
		}

		public override ScreenName ScreenStep =>  GreenFlowScreenName.RunBlueFlow;

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			return new StepData(SampleAppFlowType.BlueFlow,
				new BlueFlow.Steps.FlowInitializer.StartScreenModel
				{
					GreenFlowData = contextData.GetStepData<InitialScreen.InitialScreenScreenModel>().StepValue1,
					CallbackFlowHandler = contextData.FlowHandler,
					CallbackFlowEvent = StepEvent.BlueFlowCompleted
				});
		}

		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration,
			IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventNavigatesTo(StepEvent.BlueFlowCompleted,
				GreenFlowScreenName.FlowCompletedScreen);
		}

		protected override Task OnHandlingStepEvent(ScreenEvent triggeredEvent, IUiFlowContextData contextData)
		{
			if (triggeredEvent == StepEvent.BlueFlowCompleted)
			{
				contextData.GetCurrentStepData<StepData>().BlueFlowCompletedEventHandled = true;
			}

			return base.OnHandlingStepEvent(triggeredEvent, contextData);
		}

		/// <summary>
		/// Created to ease usage of parent class
		/// </summary>
		public class StepData : ConnectToFlow<BlueFlow.Steps.FlowInitializer.StartScreenModel, string>
		{
			public StepData(SampleAppFlowType startFlowType, BlueFlow.Steps.FlowInitializer.StartScreenModel startData = null,
				bool asContained = false) : base(startFlowType.ToString(), startData, asContained)
			{
			}

			[JsonConstructor]
			private StepData()
			{
			}

			public bool BlueFlowCompletedEventHandled { get; set; }
		}
	}
}