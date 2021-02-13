using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages.Container1;
using S3.UI.TestServices.Sut;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Tests.BlueFlow.StepC
{
	[TestFixture]
	internal abstract class
		WhenInContainerFlowStepCTests<TRootContainerPage> : ContainedFlowTestsBase<TRootContainerPage>
		where TRootContainerPage : ISutPage
	{


		protected async Task<BlueFlowStepC> ResolveSut(BlueFlowStep0 step0)
		{
			await step0.InputValues("aa", "just validate").Next();
			return AsStep<BlueFlowStepC>();
		}

		protected BlueFlowStepC PageSut { get; set; }

		public virtual (string, string)[] InputQueryStringParameters { get; } = { };


		private BlueFlowCompleted AsBlueFlowCompleted()
		{
			return App.CurrentPageAs<Container1Page0>().GetCurrentContained<BlueFlowCompleted>();
		}
		[Test]
		public async Task CanGoToNextContainerPage_AndKeepsStateWhenBack()
		{
			await App.ClickOnElementById("containerNext");
			await App.ClickOnElementById("containerPrevious");
			Assert.IsNotNull(AsStep<BlueFlowStepC>());
		}

		[Test]
		public async Task InputParametersAreCorrect()
		{
			AssertInputParameters();
		}

		protected virtual void AssertInputParameters()
		{
			Assert.AreEqual(
				$"Flow start value:{string.Join(" - ", InputQueryStringParameters.Select(x => $"{x.Item2}"))}",
				PageSut.Document.QuerySelector(" div:nth-child(1) > div > label").TextContent);
		}

		[Test]
		public async Task ShowsStep0ValueAndViewData()
		{
			Assert.AreEqual("aa", PageSut.InitialScreenValue);
			AssertViewDataValueIs(0);
			AssertInputParameters();
		}

		private void AssertViewDataValueIs(int value)
		{
			Assert.AreEqual(value,
				PageSut.ViewDataValue);
		}


		[Test]
		public async Task Complete_ShowsConfirmation()
		{
			await PageSut.Complete();
			AsBlueFlowCompleted();
		}


		[Test]
		public async Task Previous_ShowsStep0_AndChangeValue_ThenNextValueIsUpdated()
		{
			AssertInputParameters();
			await PageSut.ClickOnElementByText("Previous");
			var step0 = AsStep<BlueFlowStep0>();
			step0.Input.Value = "bb";
			await step0.ClickOnElementByText("Next");
			var stepA = AsStep<BlueFlowStepA>();
			stepA.Input.Value = "cc";

			await stepA.ClickOnElementByText("Next");
			var stepB = AsStep<BlueFlowStepB>();
			stepB.Input.Value = "dd";
			await stepB.ClickOnElementByText("Next");
			PageSut = AsStep<BlueFlowStepC>();

			Assert.AreEqual("bb", PageSut.InitialScreenValue);
			Assert.AreEqual("cc", PageSut.StepAValue);
			Assert.AreEqual("dd", PageSut.StepBValue);
			AssertInputParameters();
		}

		[TestCase(10)]
		[TestCase(20)]
		[TestCase(30)]
		public async Task ViewDataValue_IsCorrect(int value)
		{
			await PageSut.ClickOnElementByText($"Set {value}");
			PageSut = AsStep<BlueFlowStepC>();
			AssertViewDataValueIs(value);
			AssertInputParameters();
		}

		[TestCase("22")]
		[TestCase("sadf")]
		public async Task Resets_RestartsStep0(string input)
		{
			await PageSut.ClickOnElementByText("Reset");
			var step0 = AsStep<BlueFlowStep0>();
			CollectionAssert.IsEmpty(step0.Errors());
			Assert.IsEmpty(step0.Input.Value);
			Assert.AreEqual(string.Empty, step0.FieldValidatorValue);
		}
		[Test]
		public async Task CanOpenNewBlueFlow()
		{
			await PageSut.ClickOnElementByText("Open new blue flow");
			var step0 = AsStep<BlueFlowStep0>();
			CollectionAssert.IsEmpty(step0.Errors());
			Assert.IsEmpty(step0.Input.Value);
			Assert.AreEqual(string.Empty, step0.FieldValidatorValue);
		}
	}
}