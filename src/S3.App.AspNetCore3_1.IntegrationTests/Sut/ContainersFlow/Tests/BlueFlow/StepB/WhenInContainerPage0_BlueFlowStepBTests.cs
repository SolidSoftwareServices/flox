using System.Threading.Tasks;
using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages.Container1;
using NUnit.Framework;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Tests.BlueFlow.StepB
{
	[TestFixture]
	internal class WhenInContainerPage0_BlueFlowStepBTests : WhenInContainer_BlueFlowStepBTests<Container1Page0>
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