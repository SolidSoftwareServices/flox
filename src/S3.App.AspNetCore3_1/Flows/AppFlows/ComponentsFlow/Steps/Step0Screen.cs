using System.Threading.Tasks;
using S3.App.AspNetCore3_1.Flows.AppFlows.ComponentsFlow.FlowDefinitions;
using S3.App.AspNetCore3_1.Flows.AppFlows.MetadataTestFlow.FlowDefinitions;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Core.Infrastructure.DataSources;

namespace S3.App.AspNetCore3_1.Flows.AppFlows.ComponentsFlow.Steps
{
	public class Step0Screen : ComponentsFlowScreen
	{
		public override ScreenName ScreenStep =>  MetadataTestFlowScreenScreenName.Step0Screen;

		

		public override string ViewPath => "Step0";

		

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			return new InitialScreenScreenModel();
		}


		public class InitialScreenScreenModel: UiFlowScreenModel
		{
			
		}
	}
}
