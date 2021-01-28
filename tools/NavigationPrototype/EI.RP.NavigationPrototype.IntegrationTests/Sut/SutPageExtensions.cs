using EI.RP.UI.TestServices.Sut;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut
{
	public static class SutPageExtensions
	{
		public static bool IsInBlueFlow(this ISutPage src)
		{
			var flowPageId = "BlueFlowPage";
			return src.IsInFlow( flowPageId);
		}
		public static bool IsInGreenFlow(this ISutPage src)
		{
			var flowPageId = "GreenFlowPage";
			return src.IsInFlow(flowPageId);
		}

		public static bool IsInContainersFlow(this ISutPage src)
		{
			var flowPageId = "ContainersFlowPage";
			return src.IsInFlow(flowPageId);
		}

		public static bool IsInModelTesterFlowPageFlow(this ISutPage src)
		{
			var flowPageId = "ModelTesterFlowPage";
			return src.IsInFlow(flowPageId);
		}
		public static bool IsInFlow(this ISutPage src, string flowPageId)
		{
			return src.Document.QuerySelector("body > div")?.Id == flowPageId;
		}
	}
}