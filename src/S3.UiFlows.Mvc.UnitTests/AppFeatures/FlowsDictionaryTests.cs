using System.Linq;
using NUnit.Framework;
using S3.App.Flows.AppFlows.BlueFlow.Steps;
using S3.UiFlows.Mvc.AppFeatures;
using S3.UiFlows.Mvc.Infrastructure;

namespace S3.UiFlows.Mvc.UnitTests.AppFeatures
{
	public class FlowsDictionaryTests
	{
		[Test]
		public void CanScanFlows()
		{
			var expected = new[]
			{
				"BlueFlow",
				"GreenFlow", "ContainersFlow", "ContainersFlow2", "ContainersFlow3", "ContainersFlow4",
				"ModelTesterFlow", "StartFailure", "MetadataTestFlow", "ComponentsFlow"
			};
			var actual= FlowsRegistry.Instance
				.Load(typeof(InitialScreen).Assembly, "S3.App.Flows.AppFlows","/Flows/AppFlows")
				.AllFlows
				.Select(x=>x.Name);

			CollectionAssert.AreEquivalent(expected,actual);
			
		}

		[Test]
		public void CanResolveUrlPath()
		{

			var actual = FlowsRegistry.Instance
				.Load(typeof(InitialScreen).Assembly, "S3.App.Flows.AppFlows", "/Flows/AppFlows")
				.GetByName("bLuEflow");

			Assert.AreEqual("/Flows/AppFlows/BlueFlow".ToLowerInvariant(),actual.UrlPath.ToLowerInvariant());

		}
	}
}