using System.Threading.Tasks;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages.Container1;
using NUnit.Framework;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Tests.BlueFlow.Step0
{
	[TestFixture]
	internal class WhenInContainerPage0_BlueFlowStep0Tests : WhenInContainer_BlueFlowStep0Tests<Container1Page0>
	{
		protected override IContainerPage ResolveImmediateContainer()
		{
			return App.CurrentPageAs<Container1Page0>();
		}

		protected override async Task TestScenarioArrangement()
		{
			var containersPage = await App.ToContainerFlow();
			await containersPage.SelectBlueFlow();
			Assert.IsNotNull(AsStep0());
		}


	}
}