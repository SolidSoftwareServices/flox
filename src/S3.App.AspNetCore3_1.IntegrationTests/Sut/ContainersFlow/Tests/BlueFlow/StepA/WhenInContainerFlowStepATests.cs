using System.Linq;
using System.Threading.Tasks;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Pages;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Pages.Container1;
using S3.UI.TestServices.Sut;
using NUnit.Framework;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.ContainersFlow.Tests.BlueFlow.StepA
{
	[TestFixture]
	internal abstract class WhenInContainerFlowStepATests<TRootContainerPage> : ContainedFlowTestsBase<TRootContainerPage>
		where TRootContainerPage : ISutPage
	{

		protected async Task<BlueFlowStepA> ResolveSut(BlueFlowStep0 step0)
		{
			
			await step0.InputValues("ggg", "just validate")
				.ClickOnElementByText("Next");
			return AsStep<BlueFlowStepA>();
		}
		protected BlueFlowStepA PageSut { get; set; }


		[Test]
		public async Task CanGoToNextContainerPage_AndKeepsStateWhenBack()
		{
			await App.ClickOnElementById("containerNext");
			await App.ClickOnElementById("containerPrevious");
			Assert.IsNotNull(AsStep<BlueFlowStepA>());
		}

		[Test]
		public async Task AndNoInput_Next_ShowsError()
		{
			await PageSut.ClickOnElementByText("Next");
			PageSut = AsStep<BlueFlowStepA>();

			var errors = PageSut.Errors();
			CollectionAssert.IsNotEmpty(errors);

			Assert.AreEqual("Value cannot be empty", errors.Single());
		}


		[Test]
		public async Task AndInputIsOk_Next_ShowsStepB_ThenPreviousKeptInput()
		{

			PageSut.Input.Value = "Ok";
			await PageSut.ClickOnElementByText("Next");

			await AsStep<BlueFlowStepB>().ClickOnElementByText("Previous");
			PageSut = AsStep<BlueFlowStepA>();

			Assert.AreEqual("Ok", PageSut.Input.Value);
			var errors = PageSut.Errors();
			Assert.IsTrue(!errors.Any());
		}


	
		[TestCase("22")]
		[TestCase("sadf")]
		public async Task Resets_RestartsStep0(string input)
		{
			if (input != null)
			{
				PageSut.Input.Value = input;
			}
			await PageSut.ClickOnElementByText("Reset");
			var step0 = AsStep<BlueFlowStep0>();
			CollectionAssert.IsEmpty(step0.Errors());
			Assert.IsEmpty(step0.Input.Value);
			Assert.AreEqual(string.Empty, step0.FieldValidatorValue);
		}

	}
}