using System;
using System.Threading.Tasks;
using EI.RP.NavigationPrototype.IntegrationTests.Infrastructure;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.BlueFlow.Pages;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages.Container1;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Tests.BlueFlow.StepA
{
	[TestFixture]
	internal class WhenInContainerPage0_BlueFlowStepATests : WhenInContainer_BlueFlowStepATests<Container1Page0>
	{

		protected override async Task TestScenarioArrangement()
		{
			var containersPage = await App.ToContainerFlow();
			await containersPage.SelectBlueFlow();

			Sut = App.CurrentPageAs<Container1Page0>();
			var page=Sut.GetCurrentContained<BlueFlowStep0>();
			PageSut=await ResolveSut(page);
		}


		protected override IContainerPage ResolveImmediateContainer()
		{
			return App.CurrentPageAs<Container1Page0>();
		}
	}
}