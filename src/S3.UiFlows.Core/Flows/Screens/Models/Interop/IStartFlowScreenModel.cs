namespace S3.UiFlows.Core.Flows.Screens.Models.Interop
{
	internal interface IStartFlowScreenModel : IUiFlowScreenModel
	{
		string StartFlowType { get; set; }
		bool AsContained { get; set; }
		string CallbackFromFlowHandler { get; set; }
		object StartDataAsObject();
		void SetFlowResult(object result);
	}
}