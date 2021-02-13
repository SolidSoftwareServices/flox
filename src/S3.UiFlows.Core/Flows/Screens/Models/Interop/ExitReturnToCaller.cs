using Newtonsoft.Json;
using S3.UiFlows.Core.Flows.Initialization.Models;

namespace S3.UiFlows.Core.Flows.Screens.Models.Interop
{
	public class ExitReturnToCaller : UiFlowScreenModel
	{
		public ExitReturnToCaller(string callbackFlowHandler, string callbackEvent, object flowResult)
		{
			CallbackFlowHandler = callbackFlowHandler;
			CallbackEvent = callbackEvent;
			FlowResult = flowResult;
		}

		public ExitReturnToCaller(InitialFlowScreenModel src, object result) : this(src.CallbackFlowHandler,
			src.CallbackFlowEvent, result)
		{
		}

		[JsonConstructor]
		private ExitReturnToCaller()
		{
		}

		public string CallbackFlowHandler { get; set; }
		public string CallbackEvent { get; set; }
		public object FlowResult { get; set; }
	}
}