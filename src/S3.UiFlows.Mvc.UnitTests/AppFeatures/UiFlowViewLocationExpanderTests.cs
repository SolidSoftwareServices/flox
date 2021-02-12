using NUnit.Framework;
using S3.App.Flows.AppFlows.BlueFlow.Steps;
using S3.UiFlows.Mvc.AppFeatures;
using S3.UiFlows.Mvc.Infrastructure;

namespace S3.UiFlows.Mvc.UnitTests.AppFeatures
{
	public class UiFlowViewLocationExpanderTests
	{
		

		[Test]
		public void ItResolvesNotSharedFlowSpecificComponents()
		{

			var expected = new[]
			{
				"/Flows/AppFlows/BlueFlow/Components/{0}/{0}.cshtml",
				"/Flows/AppFlows/GreenFlow/Components/{0}/{0}.cshtml"
				,"/Flows/AppFlows/ContainersFlow/Components/{0}/{0}.cshtml"
				, "/Flows/AppFlows/ContainersFlow2/Components/{0}/{0}.cshtml"
				, "/Flows/AppFlows/ContainersFlow3/Components/{0}/{0}.cshtml"
				, "/Flows/AppFlows/ContainersFlow4/Components/{0}/{0}.cshtml"
				, "/Flows/AppFlows/ModelTesterFlow/Components/{0}/{0}.cshtml"
				,"/Flows/AppFlows/StartFailure/Components/{0}/{0}.cshtml"
				,"/Flows/AppFlows/MetadataTestFlow/Components/{0}/{0}.cshtml"
				,"/Flows/AppFlows/ComponentsFlow/Components/{0}/{0}.cshtml"
			};

			

			var sut = new UiFlowViewLocationExpander(new FlowsRegistry(typeof(InitialScreen).Assembly, "S3.App.Flows.AppFlows", "/Flows/AppFlows"));
			CollectionAssert.AreEquivalent(expected, sut.NotSharedFlowSpecificComponents.Value);
		}
	}
}