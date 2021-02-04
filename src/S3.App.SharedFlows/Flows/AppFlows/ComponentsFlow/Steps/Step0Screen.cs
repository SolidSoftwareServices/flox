using System.Threading.Tasks;
using S3.App.Flows.AppFlows.ComponentsFlow.FlowDefinitions;
using S3.App.Flows.AppFlows.MetadataTestFlow.FlowDefinitions;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.App.Flows.AppFlows.ComponentsFlow.Steps
{
	public class Step0Screen : ComponentsFlowScreen
	{
		public override ScreenName ScreenStep =>  MetadataTestFlowScreenScreenName.Step0Screen;

		

		public override string ViewPath => "Step0";

		

		protected override async Task<UiFlowScreenModel> OnCreateModelAsync(IUiFlowContextData contextData)
		{
			return new InitialScreenScreenModel();
		}


		public class InitialScreenScreenModel: UiFlowScreenModel
		{
			
		}
	}
}
