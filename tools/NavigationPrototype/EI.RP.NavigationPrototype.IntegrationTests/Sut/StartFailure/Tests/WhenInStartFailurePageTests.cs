using System.Threading.Tasks;
using EI.RP.NavigationPrototype.IntegrationTests.Infrastructure;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.Index.Pages;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.StartFailure.Pages;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.StartFailure.Tests
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