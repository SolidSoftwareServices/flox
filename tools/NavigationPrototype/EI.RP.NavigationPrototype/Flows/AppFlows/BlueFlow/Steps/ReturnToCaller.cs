using System.Threading.Tasks;
using EI.RP.NavigationPrototype.Flows.AppFlows.BlueFlow.FlowDefinitions;
using EI.RP.NavigationPrototype.Flows.AppFlows.BlueFlow.Steps.FillData;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Flows.Screens.Models.Interop;


namespace EI.RP.NavigationPrototype.Flows.AppFlows.BlueFlow.Steps
{
	public class ReturnToCaller : BlueFlowScreen
	{
		public override ScreenName ScreenStep =>  BlueFlowScreenName.EndAndReturnToCaller;

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var initData = contextData.GetStepData<FlowInitializer.StartScreenModel>(ScreenName.PreStart);

			var screenStepData = contextData.GetStepData<InitialScreen.InitialScreenScreenModel>();

			//TODO:Add interop capabilities
			return new CallbackOriginalFlow(initData,screenStepData.StringValue);
		}

		
	}
}