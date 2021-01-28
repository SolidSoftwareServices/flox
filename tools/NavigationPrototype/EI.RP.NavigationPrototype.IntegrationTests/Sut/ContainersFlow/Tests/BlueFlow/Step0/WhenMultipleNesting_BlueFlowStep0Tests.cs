using System.Threading.Tasks;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages.Container1;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages.Container2;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Tests.BlueFlow.Step0
{
	[TestFixture]
	internal class WhenMultipleNesting_BlueFlowStep0Tests : WhenInContainer_BlueFlowStep0Tests<Container2Page0>
	{

		protected override async Task TestScenarioArrangement()
		{
			var container2FlowPage = await App.ToContainer2Flow();
			await container2FlowPage.SelectContainerFlow();
			container2FlowPage = App.CurrentPageAs<Container2Page0>();
			
			var containersPage = container2FlowPage.GetCurrentContained<Container1Page0>();
			await containersPage.SelectBlueFlow();
			Assert.IsNotNull(AsStep0());
		}

		protected override IContainerPage ResolveImmediateContainer()
		{
			var page = App.CurrentPageAs<Container2Page0>();
			return page.GetCurrentContained<Container1Page0>();
		}

	

	}
}