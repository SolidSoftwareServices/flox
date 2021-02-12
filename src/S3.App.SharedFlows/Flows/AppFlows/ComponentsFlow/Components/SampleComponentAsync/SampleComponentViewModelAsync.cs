using S3.UiFlows.Mvc.Components;

namespace S3.App.Flows.AppFlows.ComponentsFlow.Components.SampleComponentAsync
{
	public class SampleComponentViewModelAsync: FlowComponentViewModel
	{
		public  string Value { get; set; }

		public string DataFromRestoredScreenModel{ get; set; }
	}
}