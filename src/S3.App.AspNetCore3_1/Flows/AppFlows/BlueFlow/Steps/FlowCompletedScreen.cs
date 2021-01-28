using System.Threading.Tasks;
using S3.App.AspNetCore3_1.Flows.AppFlows.BlueFlow.FlowDefinitions;
using S3.UiFlows.Core.Infrastructure.DataSources;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;


namespace S3.App.AspNetCore3_1.Flows.AppFlows.BlueFlow.Steps
{


	public class FlowCompletedScreen : BlueFlowScreen
	{
		
		public override ScreenName ScreenStep => 
			BlueFlowScreenName.FlowCompletedScreen;

		public override string ViewPath { get; } = "Completed";

		
		

		protected override async Task<UiFlowScreenModel> OnCreateStepDataAsync(IUiFlowContextData contextData)
		{
			return new FlowCompletedScreenScreenModel
			{
				
				
			};
		}

		public class FlowCompletedScreenScreenModel : UiFlowScreenModel
		{
			public override bool IsValidFor(ScreenName screenStep)
			{
					return  screenStep== BlueFlowScreenName.FlowCompletedScreen;
			}
		}
	}
}