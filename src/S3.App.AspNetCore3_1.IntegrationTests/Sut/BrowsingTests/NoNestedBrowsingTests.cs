using System.Threading.Tasks;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.Index.Pages;
using NUnit.Framework;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.BrowsingTests
{
	[TestFixture]
	class NoNestedBrowsingTests : BrowsingTests
	{
		protected override async Task TestScenarioArrangement()
		{
			var indexPage = (await App.ToIndexPage()).CurrentPageAs<IndexPage>();
			Sut = (await indexPage.SelectBlueFlow()).CurrentPageAs<BlueFlowStep0>();
		}


		public override BlueFlowStep0 AsStep0()
		{
			return App.CurrentPageAs<BlueFlowStep0>();
		}

		public override BlueFlowStepA AsStepA()
		{
			return App.CurrentPageAs<BlueFlowStepA>();
		}

		public override BlueFlowStepB AsStepB()
		{
			return App.CurrentPageAs<BlueFlowStepB>();
		}

		public override BlueFlowStepC AsStepC()
		{
			return App.CurrentPageAs<BlueFlowStepC>();
		}
	}
}