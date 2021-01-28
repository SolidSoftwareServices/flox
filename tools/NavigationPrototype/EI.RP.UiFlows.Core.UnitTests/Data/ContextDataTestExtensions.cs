using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens.Models;


namespace EI.RP.UiFlows.Core.UnitTests.Data
{
	public static class ContextDataTestExtensions
	{
		public static  UiFlowScreenModel AddStepData(this UiFlowContextData ctx, string flowScreenName,bool setCurrentStepData=false)
		{
			var userData = new UiFlowScreenModel();
			userData.Metadata.FlowHandler = ctx.FlowHandler;
			userData.Metadata.FlowScreenName = flowScreenName;
			ctx.SetStepData(flowScreenName, userData);
			if (setCurrentStepData)
			{
				ctx.CurrentScreenStep = flowScreenName;
			}
			return userData;
		}
	}
}