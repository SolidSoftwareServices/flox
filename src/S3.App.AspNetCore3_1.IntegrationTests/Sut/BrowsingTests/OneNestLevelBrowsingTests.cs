using System.Threading.Tasks;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages.Container1;
using NUnit.Framework;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.BrowsingTests
{
	[TestFixture]
	class OneNestLevelBrowsingTests : BrowsingTests
	{
		protected override async Task TestScenarioArrangement()
		{
			var containersPage = await App.ToContainerFlow();
			await containersPage.SelectBlueFlow();
			Sut = AsStep0();
			Assert.IsNotNull(Sut);
		}


		public override BlueFlowStep0 AsStep0()
		{
			return App.CurrentPageAs<Container1Page0>().GetCurrentContained<BlueFlowStep0>();
		}

		public override BlueFlowStepA AsStepA()
		{
			return App.CurrentPageAs<Container1Page0>().GetCurrentContained<BlueFlowStepA>();
		}

		public override BlueFlowStepB AsStepB()
		{
			return App.CurrentPageAs<Container1Page0>().GetCurrentContained<BlueFlowStepB>();
		}

		public override BlueFlowStepC AsStepC()
		{
			return App.CurrentPageAs<Container1Page0>().GetCurrentContained<BlueFlowStepC>();
		}
	}
}