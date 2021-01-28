using System.Threading.Tasks;
using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.GreenFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages.Container1;
using NUnit.Framework;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Tests.GreenFlow.StepC
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