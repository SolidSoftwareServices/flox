using System.Threading.Tasks;
using NUnit.Framework;
using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.Index.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.StartFailure.Pages;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.StartFailure.Tests
{
	[TestFixture]
	internal class WhenInStartFailurePageTests : WebAppPageTests<StartFailureFlowStep0>
	{
		protected override async Task TestScenarioArrangement()
		{
			var indexPage = (await App.ToIndexPage()).CurrentPageAs<IndexPage>();
			Sut = (await indexPage.SelectStartFailureFlow()).CurrentPageAs<StartFailureFlowStep0>();
		}

		[Test]
		public async Task CanBrowse()
		{
			Assert.Pass("gIVEN IS ENOUGH");
		}
	}
}