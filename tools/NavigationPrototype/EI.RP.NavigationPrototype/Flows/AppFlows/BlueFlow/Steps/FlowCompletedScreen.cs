using System.Threading.Tasks;
using EI.RP.NavigationPrototype.Flows.AppFlows.BlueFlow.FlowDefinitions;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;


namespace EI.RP.NavigationPrototype.Flows.AppFlows.BlueFlow.Steps
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