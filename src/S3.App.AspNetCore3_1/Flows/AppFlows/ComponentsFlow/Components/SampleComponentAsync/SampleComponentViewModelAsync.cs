
using S3.UiFlows.Mvc.Components;
using S3.UiFlows.Mvc.Components.Deferred;

namespace S3.App.AspNetCore3_1.Flows.AppFlows.ComponentsFlow.Components.SampleComponentAsync
{
	public class SampleComponentViewModelAsync: FlowComponentViewModel
	{
		public  string Value { get; set; }

		public string DataFromRestoredScreenModel{ get; set; }
	}
}