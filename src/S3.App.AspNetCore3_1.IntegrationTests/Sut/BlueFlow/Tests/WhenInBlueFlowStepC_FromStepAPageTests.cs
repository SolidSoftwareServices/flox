using System;
using System.Linq;
using System.Threading.Tasks;
using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.Index.Pages;
using NUnit.Framework;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Tests
{
	[TestFixture]
	class WhenInBlueFlowStepC_FromStepAPageTests : WebAppPageTests<BlueFlowStepC>
	{

		protected override async Task TestScenarioArrangement()
		{
			var queryString = string.Empty;
			if (InputQueryStringParameters.Any())
			{
				queryString = string.Join('&', InputQueryStringParameters.Select(x => $"{x.Item1}={x.Item2}"));
			}

			var indexPage = (await App.ToIndexPage()).CurrentPageAs<IndexPage>();
			var step0 = (await indexPage.SelectBlueFlow(queryString)).CurrentPageAs<BlueFlowStep0>();
			await ResolveSut(step0);
		}

		protected async Task ResolveSut(BlueFlowStep0 step0)
		{
			step0.Input.Value = "aa";
			step0.SampleInput.Value = "just validate";
			Sut = (await step0.ClickOnElementByText("Next")).CurrentPageAs<BlueFlowStepC>();
		}

		public virtual (string,string)[] InputQueryStringParameters { get; } = {};

		[Test]
		public async Task InputParametersAreCorrect()
		{
			AssertInputParameters();
		}

		protected virtual void AssertInputParameters()
		{
			Assert.AreEqual(
				$"Flow start value:{string.Join(" - ", InputQueryStringParameters.Select(x => $"{x.Item2}"))}",
				Sut.Document.QuerySelector("#blueflow- > div:nth-child(1) > div > label").TextContent);
		}

		[Test]
		public async Task ShowsStep0ValueAndViewData()
		{
			Assert.AreEqual("Initial screen value:aa", Sut.Document.QuerySelector("#blueflow- > div:nth-child(2) > div > label").TextContent);
			AssertViewDataValueIs(0);
			AssertInputParameters();
		}

		private void AssertViewDataValueIs(int value)
		{
			Assert.AreEqual($"ViewData value:{value}",
				Sut.Document.QuerySelector("#BlueFlowPage > div:nth-child(3) > div:nth-child(5) > label").TextContent);
		}


		[Test]
		public async Task Complete_ShowsConfirmation()
		{
			(await Sut.Complete()).CurrentPageAs<BlueFlowCompleted>();
		}


		[Test]
		public async Task Previous_ShowsStep0_AndChangeValue_ThenNextValueIsUpdated()
		{
			AssertInputParameters();
			var step0=(await Sut.ClickOnElementByText("Previous")).CurrentPageAs<BlueFlowStep0>();
			step0.Input.Value = "bb";
			var stepA = (await step0.ClickOnElementByText("Next")).CurrentPageAs<BlueFlowStepA>();
			stepA.Input.Value = "cc";

			var stepB = (await stepA.ClickOnElementByText("Next")).CurrentPageAs<BlueFlowStepB>();
			stepB.Input.Value = "dd";

			Sut = (await stepB.ClickOnElementByText("Next")).CurrentPageAs<BlueFlowStepC>();

			Assert.AreEqual("Initial screen value:bb", Sut.Document.QuerySelector("#blueflow- > div:nth-child(2) > div > label").TextContent);
			Assert.AreEqual("StepA value:cc", Sut.Document.QuerySelector("#blueflow- > div:nth-child(3) > div > label").TextContent);
			Assert.AreEqual("StepB value:dd", Sut.Document.QuerySelector("#blueflow- > div:nth-child(4) > div > label").TextContent);
			AssertInputParameters();
		}

		[TestCase(10)]
		[TestCase(20)]
		[TestCase(30)]
		public async Task ViewDataValue_IsCorrect(int value)
		{
			Sut=(await Sut.ClickOnElementByText($"Set {value}")).CurrentPageAs<BlueFlowStepC>();
			AssertViewDataValueIs(value);
			AssertInputParameters();
		}

		[TestCase("22")]
		[TestCase("sadf")]
		public async Task Resets_RestartsStep0(string input)
		{
			
			var step0 = (await Sut.ClickOnElementByText("Reset")).CurrentPageAs<BlueFlowStep0>();
			CollectionAssert.IsEmpty(step0.Errors());
			Assert.IsEmpty(step0.Input.Value);
			Assert.AreEqual(string.Empty, step0.FieldValidatorValue);
		}
		[Test]
		public async Task CanOpenNewBlueFlow()
		{
			var step0 = (await Sut.ClickOnElementByText("Open new blue flow")).CurrentPageAs<BlueFlowStep0>();
			CollectionAssert.IsEmpty(step0.Errors());
			Assert.IsEmpty(step0.Input.Value);
			Assert.AreEqual(string.Empty, step0.FieldValidatorValue);
		}

	}
}