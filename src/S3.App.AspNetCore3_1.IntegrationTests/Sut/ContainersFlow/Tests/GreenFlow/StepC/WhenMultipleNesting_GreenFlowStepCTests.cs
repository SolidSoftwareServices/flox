using System.Threading.Tasks;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages.Container1;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages.Container2;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Tests.GreenFlow.StepA;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.GreenFlow.Pages;
using NUnit.Framework;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Tests.GreenFlow.StepC
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
			var page = AsStep<GreenFlowStep0>();
			await ResolveSut(page);
		}
	
		protected override IContainerPage ResolveImmediateContainer()
		{
			var page = App.CurrentPageAs<Container2Page0>();
			return page.GetCurrentContained<Container1Page0>();
		}
	}
}