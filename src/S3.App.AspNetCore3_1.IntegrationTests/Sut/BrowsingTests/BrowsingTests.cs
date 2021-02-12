using System.Threading.Tasks;
using NUnit.Framework;
using S3.App.AspNetCore3_1.IntegrationTests.Infrastructure;
using S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Pages;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.BrowsingTests
{
	[TestFixture]
	abstract class BrowsingTests : WebAppPageTests<BlueFlowStep0>
	{
		public abstract BlueFlowStep0 AsStep0();
		public abstract BlueFlowStepA AsStepA();
		public abstract BlueFlowStepB AsStepB();
		public abstract BlueFlowStepC AsStepC();


		[Test]
		public async Task ItCanExecuteBasicBackAndForward()
		{
			await AsStep0().InputValues("qq", "ww").Next();
			AsStepA();
			await App.BrowsePrevious();
			AsStep0();
			await App.BrowseForward();
			AsStepA();
			await App.BrowsePrevious();
			var step0 = AsStep0();

			Assert.AreEqual("qq", step0.Input.Value);
			Assert.AreEqual("ww", step0.SampleInput.Value);
		}

		[Test]
		public async Task ItCanExecuteComplexNavigation()
		{
			await AsStep0().InputValues("qq", "ww").Next();


			await AsStepA().InputValues("pp").Next();
			await App.BrowsePrevious();
			var stepA = AsStepA();

			await App.BrowsePrevious();
			await AsStep0().InputValues("aa", "bb").Next();

			var stepC = AsStepC();

			//TODO:fix selector accessing to another tag
			//Assert.IsNull(stepC.StepAValue);


			Assert.AreEqual("aa", stepC.InitialScreenValue);
			await App.BrowsePrevious();
			var step0 = AsStep0();
			Assert.AreEqual("aa", step0.Input.Value);
			Assert.AreEqual("bb", step0.SampleInput.Value);

			await step0.InputValues("xx", "yy").Next();
			await App.BrowsePrevious();
			step0 = AsStep0();
			Assert.AreEqual("xx", step0.Input.Value);
			Assert.AreEqual("yy", step0.SampleInput.Value);
			await App.BrowseForward();

			stepA = AsStepA();
			Assert.AreEqual("", stepA.Input.Value);

		}

		[Test]
		public async Task MultipleEventsOnTheSameStepProduceTheSameResult()
		{

			var page=AsStep0().InputValues("qq", "ww");

			await page.Next();

			var originalA=AsStepA();

			var url= App.Client.Value.CurrentUrl;

			var memento = App.Options.OnlyCurrentPageCanPost;
			App.Options.OnlyCurrentPageCanPost = false;
			try
			{
				await page.Next();
			}
			finally
			{
				App.Options.OnlyCurrentPageCanPost = memento;
			}

			var actualA = AsStepA();

			Assert.AreEqual(url, App.Client.Value.CurrentUrl);
			Assert.AreNotEqual(originalA.AntiforgeryToken.Value, actualA.AntiforgeryToken.Value);

		}
	}
}