using System;
using System.Threading.Tasks;
using EI.RP.NavigationPrototype.IntegrationTests.Infrastructure;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.BlueFlow.Pages;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.Index.Pages;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.Index.Tests
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