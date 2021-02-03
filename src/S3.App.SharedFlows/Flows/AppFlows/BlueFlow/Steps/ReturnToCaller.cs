using System.Threading.Tasks;
using S3.App.Flows.AppFlows.BlueFlow.FlowDefinitions;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Core.Flows.Screens.Models.Interop;

namespace S3.App.Flows.AppFlows.BlueFlow.Steps
{
	public class ReturnToCaller : BlueFlowScreen
	{
		public override ScreenName ScreenStep =>  BlueFlowScreenName.EndAndReturnToCaller;

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			var initData = contextData.GetStepData<FlowInitializer.StartScreenModel>(ScreenName.PreStart);
			var screenStepData = contextData.GetStepData<InitialScreen.InitialScreenScreenModel>();
			
			return new ExitReturnToCaller(initData,screenStepData.StringValue);
		}

		
	}
}