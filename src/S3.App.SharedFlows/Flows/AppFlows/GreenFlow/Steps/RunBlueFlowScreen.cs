using System.Threading.Tasks;
using Newtonsoft.Json;
using S3.App.Flows.AppFlows.GreenFlow.FlowDefinitions;
using S3.UiFlows.Core.Configuration;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Core.Flows.Screens.Models.Interop;

namespace S3.App.Flows.AppFlows.GreenFlow.Steps
{
	public class RunBlueFlowScreen : UiFlowScreen
	{
		public static class ScreenInputEvent
		{
			public static readonly ScreenEvent BlueFlowCompleted = new ScreenEvent(nameof(RunBlueFlowScreen), nameof(BlueFlowCompleted));
		}

		public override ScreenName ScreenNameId =>  GreenFlowScreenName.RunBlueFlow;

		protected override async Task<UiFlowScreenModel> OnCreateModelAsync(IUiFlowContextData contextData)
		{
			return new StepData("BlueFlow",
				new BlueFlow.Steps.FlowInitializer.StartScreenModel
				{
					GreenFlowData = contextData.GetStepData<InitialScreen.InitialScreenScreenModel>().StepValue1,
					CallbackFlowHandler = contextData.FlowHandler,
					CallbackFlowEvent = ScreenInputEvent.BlueFlowCompleted
				});
		}

		protected override IScreenFlowConfigurator OnRegisterUserActions(
			IScreenFlowConfigurator screenConfiguration,
			IUiFlowContextData contextData)
		{
			return screenConfiguration
				.OnEventNavigatesTo(ScreenInputEvent.BlueFlowCompleted, GreenFlowScreenName.FlowCompletedScreen)

				.OnEventExecutes(ScreenInputEvent.BlueFlowCompleted,
					(e, ctx) => ctx.GetCurrentStepData<StepData>().BlueFlowCompletedEventHandled = true);
		}


		/// <summary>
		/// Created to ease usage of parent class
		/// </summary>
		public class StepData : StartFlowScreenModel<BlueFlow.Steps.FlowInitializer.StartScreenModel, string>
		{
			public StepData(string startFlowType, BlueFlow.Steps.FlowInitializer.StartScreenModel startData = null,
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