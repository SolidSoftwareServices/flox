using System;
using System.Linq;
using System.Threading.Tasks;
using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.GreenFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.Index.Pages;
using S3.UiFlows.Core.Flows.Screens;
using S3.UiFlows.Core.Flows.Screens.Metadata;
using NUnit.Framework;
using S3.App.Flows.AppFlows.BlueFlow.FlowDefinitions;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Tests
{


	[TestFixture]
	class WhenInBlueFlowStep0PageTests : WebAppPageTests<BlueFlowStep0>
	{
		protected override async Task TestScenarioArrangement()
		{
			var indexPage = (await App.ToIndexPage()).CurrentPageAs<IndexPage>();
			Sut = (await indexPage.SelectBlueFlow()).CurrentPageAs<BlueFlowStep0>();
		}

		[Test]
		public async Task CanSeeAsyncComponents()
		{
			Assert.IsTrue(Sut.AsyncComponent1.InnerHtml.Contains("async component input 1"));
			Assert.IsTrue(Sut.AsyncComponent2.InnerHtml.Contains("async component input 2"));
		}

		[Test]
		public async Task AndNoInput_Next_ShowsError()
		{
			Sut.SampleInput.Value = "22";
			Sut = (await Sut.Next()).CurrentPageAs<BlueFlowStep0>();
			var errors = Sut.Errors();
			CollectionAssert.IsNotEmpty(errors);

			Assert.AreEqual("StepValue1 is required", errors.Single());
			Assert.AreEqual(errors.Single(), Sut.FieldValidatorValue);
		}

		[Test]
		public async Task AndNoComponentInput_Next_ShowsError()
		{
			Sut = (await Sut.Next()).CurrentPageAs<BlueFlowStep0>();
			var errors = Sut.Errors().ToArray();
			Assert.AreEqual(2, errors.Length);

			Assert.AreEqual("StepValue1 is required", errors[0]);
			Assert.AreEqual("ERROR TEST VALUE", errors[1]);
			Assert.AreEqual(errors[0], Sut.FieldValidatorValue);
		}

		[Test]
		public async Task AndInputIsNumeric_Next_ShowsError()
		{
			Sut.SampleInput.Value = "22";
			Sut.Input.Value = "22";
			Sut = (await Sut.Next()).CurrentPageAs<BlueFlowStep0>();
			var errors = Sut.Errors();
			CollectionAssert.IsNotEmpty(errors);
			Assert.AreEqual(string.Empty, Sut.FieldValidatorValue);
			Assert.AreEqual("Numeric only value  are not allowed", errors.Single());
		}

		[Test]
		public async Task AndInputIsA_Next_ShowsStepC_ThenPreviousKeptInput()
		{
			await Sut.InputValues("a", "22").Next();
			var stepC = App.CurrentPageAs<BlueFlowStepC>();

			Sut = (await stepC.Previous())
				.CurrentPageAs<BlueFlowStep0>();

			Assert.AreEqual("a", Sut.Input.Value);
			var errors = Sut.Errors();
			Assert.IsTrue(!errors.Any());
			Assert.AreEqual(string.Empty, Sut.FieldValidatorValue);
		}

		[Test]
		public async Task AndInputIsAnother_Next_ShowsStepA_ThenPreviousKeptInput()
		{
			var blueFlowStepA = (await Sut.InputValues("sdfa", "22").Next())
				.CurrentPageAs<BlueFlowStepA>();
			Sut = (await blueFlowStepA.Previous())
				.CurrentPageAs<BlueFlowStep0>();

			Assert.AreEqual("sdfa", Sut.Input.Value);
			Assert.AreEqual("22", Sut.SampleInput.Value);
			var errors = Sut.Errors();
			Assert.IsTrue(!errors.Any());
			Assert.AreEqual(string.Empty, Sut.FieldValidatorValue);
		}

		[TestCase("")]
		[TestCase("22")]
		[TestCase("sadf")]
		public async Task Resets_RestartsStep0(string input)
		{
			if (input != null)
			{
				Sut.Input.Value = input;
			}

			Sut = (await Sut.Reset()).CurrentPageAs<BlueFlowStep0>();
			CollectionAssert.IsEmpty(Sut.Errors());
			Assert.IsEmpty(Sut.Input.Value);
			Assert.AreEqual(string.Empty, Sut.FieldValidatorValue);
		}

		[Test]
		public async Task OpenGreenFlow_ShowsTheFlow()
		{
			Assert.IsNotNull((await Sut.ClickOnOpenSibling()).CurrentPageAs<GreenFlowStep0>());
		}

		[Test]
		public async Task FailInitialisationShowsErrorPage()
		{
			var errorPage = (await Sut.ClickFailInitialisation()).CurrentPageAs<BlueFlowErrorScreen>();
			Assert.IsNotNull(errorPage);
			Assert.AreEqual("Triggering failure on FlowInitializer", errorPage.Error);
			Assert.AreEqual(ScreenName.PreStart.ToString(), errorPage.Step);
			Assert.AreEqual(ScreenLifecycleStage.FlowInitialization.ToString(), errorPage.OnLifecycleEvent);
		}

		[Test]
		public async Task FailCreationShowsErrorPage()
		{
			var errorPage = (await Sut.ClickFailCreatingScreen()).CurrentPageAs<BlueFlowErrorScreen>();
			Assert.IsNotNull(errorPage);
			Assert.AreEqual($"Failing on {BlueFlowScreenName.Step0Screen}.{ScreenLifecycleStage.CreatingStepData}", errorPage.Error);
			Assert.AreEqual(BlueFlowScreenName.Step0Screen.ToString(), errorPage.Step);
			Assert.AreEqual(ScreenLifecycleStage.CreatingStepData.ToString(), errorPage.OnLifecycleEvent);
		}
	}
}