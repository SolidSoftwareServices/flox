using System.Linq;
using System.Threading.Tasks;
using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.GreenFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.Index.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ModelTesterFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.StartFailure.Pages;
using NUnit.Framework;
using S3.App.Flows.AppFlows;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.ModelTesterFlow.Tests
{
	[TestFixture]
	internal class WhenInModelTester0PageTests : WebAppPageTests<ModelTesterPage0>
	{
		protected override async Task TestScenarioArrangement()
		{
			var indexPage = (await App.ToIndexPage()).CurrentPageAs<IndexPage>();
			Sut = (await indexPage.SelectModelTesterFlow()).CurrentPageAs<ModelTesterPage0>();
		}

		[Test]
		public async Task AndChangedInput_Next_ThenBack_KeepsValues()
		{
			var generatedValues = ModelTesterPage0.GenerateRandomValues().Select(x => (x.Key, x.Value)).ToArray();
			Sut.SetValues(generatedValues);
			var step1 = (await Sut.ClickOnElementByText("Next")).CurrentPageAs<ModelTesterCompleted>();
			Sut = (await step1.ClickOnElementByText("Back")).CurrentPageAs<ModelTesterPage0>();

			Sut.AssertValues(generatedValues);
		}

		[Test]
		public async Task AndInvalidEvent_ShowsError()
		{
			Sut.SampleInput.Value = "22";
			Sut = (await Sut.SubmitInvalidEvent()).CurrentPageAs<ModelTesterPage0>();

			var errors = Sut.Errors().ToArray();
			CollectionAssert.IsNotEmpty(errors);
			Assert.AreEqual(
				"No valid leaving transitions are permitted from state 'InputScreen' for trigger 'AnInvalidScreen.AnInvalidEvent'. Consider ignoring the trigger.",
				errors.Single());
		}

		[Test]
		public async Task AndNoInput_Next_ShowsLastStep()
		{
			Sut.SampleInput.Value = "22";
			(await Sut.ClickOnElementByText("Next")).CurrentPageAs<ModelTesterCompleted>();
		}
		[Test]
		public async Task AndNoInput_InSampleInput_ShowsErrors()
		{
			var current=(await Sut.ClickOnElementByText("Next")).CurrentPageAs<ModelTesterPage0>();
			var errors = current.Errors().ToArray();
			Assert.AreEqual(1, errors.Length);
			Assert.AreEqual("ERROR TEST VALUE", errors[0]);
			
			Assert.AreEqual(string.Empty, current.SampleInput.Value);
		}

		[Test]
		public async Task AndWithoutRequired_ShowsErrors()
		{
			Sut.RequiredIf.Value = string.Empty;
			Sut.Nested1_Level1.Value = string.Empty;
			Sut = (await Sut.ClickOnElementByText("Next")).CurrentPageAs<ModelTesterPage0>();
			var errors = Sut.Errors().ToArray();
			Assert.AreEqual(2, errors.Length);
			
			Assert.AreEqual(string.Empty, Sut.RequiredIf.Value);
			Assert.AreEqual(string.Empty, Sut.SampleInput.Value);

			Assert.AreEqual("Required", errors[0]);
			Assert.AreEqual("ERROR TEST VALUE", errors[1]);

		}

		[Test]
		public async Task AndWithRequired_ShowsLastStep()
		{
			Sut.RequiredIf.Value = "22";
			Sut.SampleInput.Value = "22";
			Sut.Nested1_Level1.Value = string.Empty;
			(await Sut.ClickOnElementByText("Next")).CurrentPageAs<ModelTesterCompleted>();
		}


		[Test]
		public async Task OpenBlueFlow_ShowsTheFlow()
		{
			Assert.IsNotNull((await Sut.ClickOnOpenSibling(SampleAppFlowType.BlueFlow)).CurrentPageAs<BlueFlowStep0>());
		}

		[Test]
		public async Task OpenGreenFlow_ShowsTheFlow()
		{
			Assert.IsNotNull(
				(await Sut.ClickOnOpenSibling(SampleAppFlowType.GreenFlow)).CurrentPageAs<GreenFlowStep0>());
		}

		[Test]
		public async Task OpenStartFailureFlow_ShowsTheFlow()
		{
			Assert.IsNotNull((await Sut.ClickOnOpenSibling(SampleAppFlowType.StartFailure))
				.CurrentPageAs<StartFailureFlowStep0>());
		}
	}
}