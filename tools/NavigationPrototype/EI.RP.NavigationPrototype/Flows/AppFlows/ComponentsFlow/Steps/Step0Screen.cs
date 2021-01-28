using System.Threading.Tasks;
using EI.RP.NavigationPrototype.Flows.AppFlows.ComponentsFlow.FlowDefinitions;
using EI.RP.NavigationPrototype.Flows.AppFlows.MetadataTestFlow.FlowDefinitions;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;

namespace EI.RP.NavigationPrototype.Flows.AppFlows.ComponentsFlow.Steps
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
