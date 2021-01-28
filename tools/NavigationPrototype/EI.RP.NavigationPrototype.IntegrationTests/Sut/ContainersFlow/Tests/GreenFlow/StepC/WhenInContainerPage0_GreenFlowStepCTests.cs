using System.Threading.Tasks;
using EI.RP.NavigationPrototype.IntegrationTests.Infrastructure;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.GreenFlow.Pages;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages.Container1;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Tests.GreenFlow.StepC
{
	[TestFixture]
	internal class WhenInContainerPage0_GreenFlowStepCTests : WhenInContainer_GreenFlowStepCTests<Container1Page0>
	{
		
		protected override async Task TestScenarioArrangement()
		{

			var containersPage = await App.ToContainerFlow();
			//await containersPage.SelectGreenFlow();

			Sut = App.CurrentPageAs<Container1Page0>();
			var page = Sut.GetCurrentContained<GreenFlowStep0>();
			PageSut = await ResolveSut(page);
		}
		protected override IContainerPage ResolveImmediateContainer()
		{
			return App.CurrentPageAs<Container1Page0>();
		}



	}
}