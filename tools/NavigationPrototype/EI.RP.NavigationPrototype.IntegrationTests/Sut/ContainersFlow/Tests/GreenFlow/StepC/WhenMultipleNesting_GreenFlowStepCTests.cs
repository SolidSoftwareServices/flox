using System.Threading.Tasks;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages.Container1;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages.Container2;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Tests.GreenFlow.StepA;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.GreenFlow.Pages;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Tests.GreenFlow.StepC
{
	[TestFixture]
	internal class WhenMultipleNesting_GreenFlowStepCTests : WhenInContainer_GreenFlowStepCTests<Container1Page0>
	{

		protected override async Task TestScenarioArrangement()
		{
			var container2FlowPage = await App.ToContainer2Flow();
			await container2FlowPage.SelectContainerFlow();
			container2FlowPage = App.CurrentPageAs<Container2Page0>();

			var containersPage = container2FlowPage.GetCurrentContained<Container1Page0>();
			//await containersPage.SelectGreenFlow();
			var page = AsStep0();
			await ResolveSut(page);
		}
	
		protected override IContainerPage ResolveImmediateContainer()
		{
			var page = App.CurrentPageAs<Container2Page0>();
			return page.GetCurrentContained<Container1Page0>();
		}
	}
}