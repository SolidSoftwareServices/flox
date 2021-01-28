using System.Linq;
using System.Threading.Tasks;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.BlueFlow.Pages;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Pages.Container1;
using EI.RP.UI.TestServices.Sut;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.ContainersFlow.Tests.BlueFlow.StepA
{
	[TestFixture]
	internal abstract class WhenInContainer_BlueFlowStepATests<TRootContainerPage> : ContainedBlueFlowTestsBase<TRootContainerPage>
		where TRootContainerPage : ISutPage
	{

		protected async Task<BlueFlowStepA> ResolveSut(BlueFlowStep0 step0)
		{
			
			await step0.InputValues("ggg", "just validate")
				.ClickOnElementByText("Next");
			return AsStepA();
		}
		protected BlueFlowStepA PageSut { get; set; }


		[Test]
		public async Task CanGoToNextContainerPage_AndKeepsStateWhenBack()
		{
			await App.ClickOnElementById("containerNext");
			await App.ClickOnElementById("containerPrevious");
			Assert.IsNotNull(AsStepA());
		}

		[Test]
		public async Task AndNoInput_Next_ShowsError()
		{
			await PageSut.ClickOnElementByText("Next");
			PageSut = AsStepA();

			var errors = PageSut.Errors();
			CollectionAssert.IsNotEmpty(errors);

			Assert.AreEqual("Value cannot be empty", errors.Single());
		}


		[Test]
		public async Task AndInputIsOk_Next_ShowsStepB_ThenPreviousKeptInput()
		{

			PageSut.Input.Value = "Ok";
			await PageSut.ClickOnElementByText("Next");

			await AsStepB().ClickOnElementByText("Previous");
			PageSut = AsStepA();

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
			var step0 = AsStep0();
			CollectionAssert.IsEmpty(step0.Errors());
			Assert.IsEmpty(step0.Input.Value);
			Assert.AreEqual(string.Empty, step0.FieldValidatorValue);
		}

	}
}