using Ei.Rp.Web.DebugTools.Flows.AppFlows.Components.FlowViewer;

namespace Ei.Rp.Web.DebugTools.Areas.FlowDebugger.Models
{
    public class DebuggerModel: FlowViewerComponent.IComponentInput
	{
	    public bool HasFlow() => !string.IsNullOrWhiteSpace(FlowHandler);
		public string FlowHandler { get; set; }
	    public string Error { get; set; }
    }
}
