using System;
using System.Threading.Tasks;
using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.Index.Pages;
using NUnit.Framework;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.Index.Tests
{
	[TestFixture]
	class IndexPageTests : WebAppPageTests<IndexPage>
	{
		protected override Task TestScenarioArrangement()
		{
			return Task.CompletedTask;
		}

		[Test]
		public async Task CurrentPageFailsWhenBadlyCast()
		{
			Assert.ThrowsAsync<InvalidCastException>(async ()=>(await App.ToIndexPage()).CurrentPageAs<BlueFlowStep0>());
		}

		[Test]
		public async Task CurrentPageWorksCorrectly()
		{
			Assert.DoesNotThrowAsync(async () => (await App.ToIndexPage()).CurrentPageAs<IndexPage>());
		}
	}
}