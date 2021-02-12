using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using S3.App.Flows.AppFlows.ContainersFlow3.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Core.Flows.Screens.Models.Interop;

namespace S3.App.Flows.AppFlows.ContainersFlow3.Steps
{
	public class Number1ContainerScreen : ContainersFlow3Screen
	{
		public override ScreenName ScreenStep =>  ContainersFlow3ScreenName.Number1ContainerScreen;
		public static class StepEvent
		{
			public static readonly ScreenEvent ToStep2 = new ScreenEvent(nameof(Number1ContainerScreen),nameof(ToStep2));
		}
		protected override IScreenFlowConfigurator OnRegisterUserActions(
			IScreenFlowConfigurator screenConfiguration, IUiFlowContextData contextData)
		{
			return screenConfiguration.OnEventReentriesCurrent(ScreenEvent.ErrorOccurred)
				.OnEventNavigatesTo(StepEvent.ToStep2, ContainersFlow3ScreenName.Number2ContainerScreen);
		}

		protected override async Task<UiFlowScreenModel> OnCreateModelAsync(IUiFlowContextData contextData)
		{
			return new StepData("BlueFlow",
				new BlueFlow.Steps.FlowInitializer.StartScreenModel
				{
					CallbackFlowHandler = contextData.FlowHandler,
					CallbackFlowEvent = StepEvent.ToStep2,

				},asContained:true);

		}
		/// <summary>
		/// Created to ease usage of parent class
		/// </summary>
		public class StepData : StartFlowScreenModel<BlueFlow.Steps.FlowInitializer.StartScreenModel, string>
		{
			public StepData(string startFlowType, BlueFlow.Steps.FlowInitializer.StartScreenModel startData = null,
				bool asContained = false) : base(startFlowType, startData, asContained)
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