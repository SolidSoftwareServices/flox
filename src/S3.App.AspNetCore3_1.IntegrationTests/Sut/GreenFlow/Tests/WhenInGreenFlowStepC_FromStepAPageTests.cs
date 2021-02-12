using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.GreenFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.Index.Pages;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.GreenFlow.Tests
{
	[TestFixture]
	class WhenInGreenFlowStepC_FromStepAPageTests : WebAppPageTests<GreenFlowStepC>
	{

		protected override async Task TestScenarioArrangement()
		{
			async Task AUserIsInTheAccountSelectionPage()
			{
				var queryString = string.Empty;
				if (InputQueryStringParameters.Any())
				{
					queryString = string.Join('&', InputQueryStringParameters.Select(x => $"{x.Item1}={x.Item2}"));
				}
				var indexPage = (await App.ToIndexPage()).CurrentPageAs<IndexPage>();
				var step0 = (await indexPage.SelectGreenFlow(queryString)).CurrentPageAs<GreenFlowStep0>();
				step0.Input.Value = "aa";
				Sut = (await step0.ClickOnElementByText("Next")).CurrentPageAs<GreenFlowStepC>();
				
			}

			await AUserIsInTheAccountSelectionPage();
		}

		public virtual (string,string)[] InputQueryStringParameters { get; } = {};

		[Test]
		public async Task InputParametersAreCorrect()
		{
			AssertInputParameters();
		}

		private void AssertInputParameters()
		{
			Assert.AreEqual(
				$"Flow start value:{string.Join(" - ", InputQueryStringParameters.Select(x => $"{x.Item2}"))}",
				Sut.Document.QuerySelector("#greenflow- > div:nth-child(1) > div > label").TextContent);
		}

		[Test]
		public async Task ShowsStep0ValueAndViewData()
		{
			Assert.AreEqual("Initial screen value:aa", Sut.Document.QuerySelector("#greenflow- > div:nth-child(2) > div > label").TextContent);
			AssertInputParameters();
		}



		[Test]
		public async Task Complete_ShowsConfirmation()
		{
			(await Sut.Complete()).CurrentPageAs<GreenFlowCompleted>();
		}


		[Test]
		public async Task Previous_ShowsStep0_AndChangeValue_ThenNextValueIsUpdated()
		{
			AssertInputParameters();
			var step0=(await Sut.ClickOnElementByText("Previous")).CurrentPageAs<GreenFlowStep0>();
			step0.Input.Value = "bb";
			var stepA = (await step0.ClickOnElementByText("Next")).CurrentPageAs<GreenFlowStepA>();
			stepA.Input.Value = "cc";

			var stepB = (await stepA.ClickOnElementByText("Next")).CurrentPageAs<GreenFlowStepB>();
			stepB.Input.Value = "dd";

			Sut = (await stepB.ClickOnElementByText("Next")).CurrentPageAs<GreenFlowStepC>();

			Assert.AreEqual("Initial screen value:bb", Sut.Document.QuerySelector("#greenflow- > div:nth-child(2) > div > label").TextContent);
			Assert.AreEqual("StepA value:cc", Sut.Document.QuerySelector("#greenflow- > div:nth-child(3) > div > label").TextContent);
			Assert.AreEqual("StepB value:dd", Sut.Document.QuerySelector("#greenflow- > div:nth-child(4) > div > label").TextContent);
			AssertInputParameters();
		}

		[TestCase("22")]
		[TestCase("sadf")]
		public async Task Resets_RestartsStep0(string input)
		{
			
			var step0 = (await Sut.Reset()).CurrentPageAs<GreenFlowStep0>();
			CollectionAssert.IsEmpty(step0.Errors());
			Assert.IsEmpty(step0.Input.Value);
			Assert.AreEqual(string.Empty, step0.FieldValidatorValue);
		}


		[Test]
		public async Task CanStartBlueFlowAndCollectResult()
		{
			var expected = DateTime.UtcNow.Ticks.ToString();
			await Sut.StartBlueFlow();
			var s0 = App.CurrentPageAs<BlueFlowStep0>();
			s0.InputValues("a", expected);
			await s0.Next();
			var sc = App.CurrentPageAs<BlueFlowStepC>();
			await sc.Complete();
			var resultPage = App.CurrentPageAs<GreenFlowCompleted>();

			Assert.AreEqual(expected,resultPage.BlueFlowValue.Value);
			Assert.IsTrue(bool.Parse(resultPage.BlueFlowEventHandled.Value));
		}


	}
}