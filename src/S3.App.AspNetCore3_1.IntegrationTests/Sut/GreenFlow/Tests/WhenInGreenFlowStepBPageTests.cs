using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.GreenFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.Index.Pages;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.GreenFlow.Tests
{
	[TestFixture]
	class WhenInGreenFlowStepBPageTests : WebAppPageTests<GreenFlowStepB>
	{

		protected override async Task TestScenarioArrangement()
		{
			async Task AUserIsInTheAccountSelectionPage()
			{
				var indexPage = (await App.ToIndexPage()).CurrentPageAs<IndexPage>();
				var step0 = (await indexPage.SelectGreenFlow()).CurrentPageAs<GreenFlowStep0>();
				step0.Input.Value = "ggg";
				var stepA = (await step0.ClickOnElementByText("Next")).CurrentPageAs<GreenFlowStepA>();
				stepA.Input.Value = "AAA";
				Sut = (await stepA.ClickOnElementByText("Next")).CurrentPageAs<GreenFlowStepB>();
			}

			await AUserIsInTheAccountSelectionPage();
		}
		[Test]
		public async Task AndNoInput_Next_ShowsError()
		{
			Sut = (await Sut.ClickOnElementByText("Next")).CurrentPageAs<GreenFlowStepB>();
			var errors = Sut.Errors();
			CollectionAssert.IsNotEmpty(errors);

			Assert.AreEqual("Value cannot be empty", errors.Single());
		}
		

		[Test]
		public async Task AndInputIsOk_Next_ShowsStepC_ThenPreviousKeptInput()
		{
			Sut.Input.Value = "Ok";
			Sut = (await (await Sut.ClickOnElementByText("Next"))
					.CurrentPageAs<GreenFlowStepC>()
					.ClickOnElementByText("Previous"))
				.CurrentPageAs<GreenFlowStepB>();

			Assert.AreEqual("Ok", Sut.Input.Value);
			var errors = Sut.Errors();
			Assert.IsTrue(!errors.Any());
		}


		[TestCase("22")]
		[TestCase("sadf")]
		public async Task Resets_RestartsStep0(string input)
		{
			if (input != null)
			{
				Sut.Input.Value = input;
			}
			var step0 = (await Sut.ClickOnElementByText("Reset")).CurrentPageAs<GreenFlowStep0>();
			CollectionAssert.IsEmpty(step0.Errors());
			Assert.IsEmpty(step0.Input.Value);
			Assert.AreEqual(string.Empty, step0.FieldValidatorValue);
		}


	}
}