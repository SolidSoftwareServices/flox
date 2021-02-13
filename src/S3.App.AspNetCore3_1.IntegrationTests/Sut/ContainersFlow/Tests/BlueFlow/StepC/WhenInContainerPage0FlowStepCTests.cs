using System.Threading.Tasks;
using NUnit.Framework;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages.Container1;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Tests.BlueFlow.StepC
{
	[TestFixture]
	internal class WhenInContainerPage0FlowStepCTests : WhenInContainerFlowStepCTests<Container1Page0>
	{
		
		protected override async Task TestScenarioArrangement()
		{

			var containersPage = await App.ToContainerFlow();
			await containersPage.SelectBlueFlow();

			Sut = App.CurrentPageAs<Container1Page0>();
			var page = Sut.GetCurrentContained<BlueFlowStep0>();
			PageSut = await ResolveSut(page);
		}
		protected override IContainerPage ResolveImmediateContainer()
		{
			return App.CurrentPageAs<Container1Page0>();
		}



	}
}