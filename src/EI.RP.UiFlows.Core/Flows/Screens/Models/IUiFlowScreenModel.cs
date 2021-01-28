using System.Collections.Generic;
using EI.RP.UiFlows.Core.Flows.Screens.ErrorHandling;

namespace EI.RP.UiFlows.Core.Flows.Screens.Models
{
	public interface IUiFlowScreenModel
	{
		string FlowHandler { get; }
		string FlowScreenName { get; }
		TScreenFlow GetFlowType<TScreenFlow>() where TScreenFlow : struct;
		IEnumerable<UiFlowUserInputError> Errors { get; set; }

		IEnumerable<ScreenEvent> DontValidateEvents { get; }

		bool IsValidFor(ScreenName screenStep);

		void SetContainedFlow<TScreenFlow>(TScreenFlow newContainedFlow, string containedFlowStartType = null);
		TScreenFlow? GetContainedFlow<TScreenFlow>() where TScreenFlow : struct;
		void ClearContainedFlow();

		string GetContainedFlowStartType();

		string GetContainedFlowHandler();

		string ScreenTitle { get; set; }
	}
}