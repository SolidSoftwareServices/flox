using System.Threading.Tasks;
using S3.App.Flows.AppFlows.BlueFlow.FlowDefinitions;
using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.App.Flows.AppFlows.BlueFlow.Steps
{


	public class FlowCompletedScreen : UiFlowScreen
	{
		
		public override ScreenName ScreenNameId => 
			BlueFlowScreenName.FlowCompletedScreen;

		public override string ViewPath { get; } = "Completed";

		
		

		protected override async Task<UiFlowScreenModel> OnCreateModelAsync(IUiFlowContextData contextData)
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