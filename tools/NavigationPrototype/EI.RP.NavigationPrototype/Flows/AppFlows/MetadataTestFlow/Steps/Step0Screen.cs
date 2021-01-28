using System.Threading.Tasks;
using EI.RP.NavigationPrototype.Flows.AppFlows.MetadataTestFlow.FlowDefinitions;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using EI.RP.UiFlows.Core.Flows.Screens.Models;


namespace EI.RP.NavigationPrototype.Flows.AppFlows.MetadataTestFlow.Steps
{
	public class Step0Screen : MetadataTestFlowScreen
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
