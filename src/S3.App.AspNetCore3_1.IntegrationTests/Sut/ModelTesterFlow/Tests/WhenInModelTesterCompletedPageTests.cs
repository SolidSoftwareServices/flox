using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.Index.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ModelTesterFlow.Pages;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.ModelTesterFlow.Tests
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

			for (var i = 0; i < 3; i++)
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