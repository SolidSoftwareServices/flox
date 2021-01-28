using System.Threading.Tasks;
using EI.RP.NavigationPrototype.Flows.AppFlows.BlueFlow.FlowDefinitions;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;


namespace EI.RP.NavigationPrototype.Flows.AppFlows.BlueFlow.Steps
{
	public class ErrorScreen : BlueFlowScreen
	{
		public override ScreenName ScreenStep => BlueFlowScreenName.ErrorScreen;

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var result = new ErrorScreenModel();
			result.Error = contextData.LastError.ExceptionMessage;
			result.OnStep = contextData.LastError.OccurredOnStep;
			result.OnLifecycleEvent = contextData.LastError.LifecycleStage.ToString();

			//error handled
			contextData.LastError = null;

			return result;
		}

		public class ErrorScreenModel : UiFlowScreenModel
		{
			public string Error { get; set; }
			public ScreenName OnStep { get; set; }
			public string OnLifecycleEvent { get; set; }

			public override bool IsValidFor(ScreenName screenStep)
			{
				return screenStep == BlueFlowScreenName.ErrorScreen;
			}
		}
	}
}