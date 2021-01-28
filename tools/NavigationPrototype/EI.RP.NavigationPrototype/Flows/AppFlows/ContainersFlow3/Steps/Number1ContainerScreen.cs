using System;
using System.Threading.Tasks;
using EI.RP.NavigationPrototype.Flows.AppFlows.ContainersFlow3.FlowDefinitions;
using EI.RP.UiFlows.Core.Configuration;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Flows.Screens.Models.Interop;

using Newtonsoft.Json;

namespace EI.RP.NavigationPrototype.Flows.AppFlows.ContainersFlow3.Steps
{
	public class Number1ContainerScreen : ContainersFlow3Screen
	{
		public override ScreenName ScreenStep =>  ContainersFlow3ScreenName.Number1ContainerScreen;
		public static class StepEvent
		{
			public static readonly ScreenEvent ToStep2 = new ScreenEvent(nameof(Number1ContainerScreen),nameof(ToStep2));
		}
		protected override IScreenFlowConfigurator OnDefiningTransitionsFromCurrentScreen(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(StepEvent.ToStep2, ContainersFlow3ScreenName.Number2ContainerScreen);
		}

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			return new StepData(SampleAppFlowType.BlueFlow,
				new BlueFlow.Steps.FlowInitializer.StartScreenModel
				{
					CallbackFlowHandler = contextData.FlowHandler,
					CallbackFlowEvent = StepEvent.ToStep2,

				},asContained:true);

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

			public DateTime InitialLoad { get; set; } = DateTime.UtcNow;
		}
	}
}