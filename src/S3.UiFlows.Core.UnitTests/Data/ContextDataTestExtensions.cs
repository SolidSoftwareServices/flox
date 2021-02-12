using S3.UiFlows.Core.DataSources;
using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.UiFlows.Core.UnitTests.Data
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