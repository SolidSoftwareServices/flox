using System.Linq;
using System.Threading.Tasks;
using EI.RP.NavigationPrototype.IntegrationTests.Infrastructure;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.Index.Pages;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ModelTesterFlow.Pages;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.ModelTesterFlow.Tests
{
	[TestFixture]
	class WhenInModelTesterCompletedPageTests : WebAppPageTests<ModelTesterCompleted>
	{
		private (string, string)[] _generatedValues;

		protected override async Task TestScenarioArrangement()
		{
			var indexPage = (await App.ToIndexPage()).CurrentPageAs<IndexPage>();
			var input = (await indexPage.SelectModelTesterFlow()).CurrentPageAs<ModelTesterPage0>();
			_generatedValues = ModelTesterPage0.GenerateRandomValues().Select(x => (x.Key, x.Value)).ToArray();
			input.SetValues(_generatedValues);

			Sut = (await input.ClickOnElementByText("Next")).CurrentPageAs<ModelTesterCompleted>();
		}

		[Test]
		public async Task ShowsDataCorrectly()
		{
			Sut.AssertValues(_generatedValues);

			for (int i = 0; i < 3; i++)
			{

				var step0 = (await Sut.ClickOnElementByText("Back")).CurrentPageAs<ModelTesterPage0>();
				step0.AssertValues(_generatedValues);
				_generatedValues = ModelTesterPage0.GenerateRandomValues().Select(x => (x.Key, x.Value)).ToArray();
				Sut = (await step0.SetValues(_generatedValues).ClickOnElementByText("Next"))
					.CurrentPageAs<ModelTesterCompleted>();
				Sut.AssertValues(_generatedValues);
			}
		}
	}
}