using System.Collections.Generic;
using S3.UiFlows.Core.Flows.Screens.ErrorHandling;

namespace S3.UiFlows.Core.Flows.Screens.Models
{
	public interface IUiFlowScreenModel
	{
		string FlowHandler { get; }
		string FlowScreenName { get; }
		IEnumerable<UiFlowUserInputError> Errors { get; set; }

		IEnumerable<ScreenEvent> DontValidateEvents { get; }

		bool IsValidFor(ScreenName screenStep);
		string GetFlowType();
		void SetContainedFlow(string newContainedFlow, string containedFlowStartType = null);
		string GetContainedFlow();
		void ClearContainedFlow();

		string GetContainedFlowStartType();

		string GetContainedFlowHandler();

		string ScreenTitle { get; set; }
	}
}