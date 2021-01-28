

namespace S3.App.AspNetCore3_1.IntegrationTests.Routing.Misc
{
	class UiFlowRoutingResult
	{
		public string Controller { get; set; }
		public string Action { get; set; }

		public string RequestedUrl { get; set; }

		public string[] RequestArgs { get; set; }

		public string FlowType { get; set; }
	}
}