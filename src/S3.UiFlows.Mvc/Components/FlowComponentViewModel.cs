using S3.UiFlows.Core.Flows.Screens.Models;

namespace S3.UiFlows.Mvc.Components
{
	public abstract class FlowComponentViewModel
	{
		public IUiFlowScreenModel ScreenModel { get; set; }
		public string ComponentId { get; set; }
	}
}