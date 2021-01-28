using EI.RP.UiFlows.Core.Flows.Initialization.Models;
using Newtonsoft.Json;

namespace EI.RP.UiFlows.Core.Flows.Screens.Models.Interop
{
	public class CallbackOriginalFlow : UiFlowScreenModel
	{
		public CallbackOriginalFlow(string callbackFlowHandler, string callbackEvent, object flowResult)
		{
			CallbackFlowHandler = callbackFlowHandler;
			CallbackEvent = callbackEvent;
			FlowResult = flowResult;
		}

		public CallbackOriginalFlow(InitialFlowScreenModel src, object result) : this(src.CallbackFlowHandler,
			src.CallbackFlowEvent, result)
		{
		}

		[JsonConstructor]
		private CallbackOriginalFlow()
		{
		}

		public string CallbackFlowHandler { get; set; }
		public string CallbackEvent { get; set; }
		public object FlowResult { get; set; }
	}
}