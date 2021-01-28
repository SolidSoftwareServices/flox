using System;
using System.Linq;
using System.Threading.Tasks;
using EI.RP.NavigationPrototype.IntegrationTests.Infrastructure;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.GreenFlow.Pages;
using EI.RP.NavigationPrototype.IntegrationTests.Sut.Index.Pages;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.GreenFlow.Tests
{
	[TestFixture]
	class WhenInGreenFlowStep0PageTests : WebAppPageTests<GreenFlowStep0>
	{

		protected override async Task TestScenarioArrangement()
		{
			async Task AUserIsInTheAccountSelectionPage()
			{
				var indexPage = (await App.ToIndexPage()).CurrentPageAs<IndexPage>();
				Sut = (await indexPage.SelectGreenFlow()).CurrentPageAs<GreenFlowStep0>();
			}

			await AUserIsInTheAccountSelectionPage();
		}
		[Test]
		public async Task AndNoInput_Next_ShowsError()
		{
			Sut = (await Sut.ClickOnElementByText("Next")).CurrentPageAs<GreenFlowStep0>();
			var errors = Sut.Errors();
			CollectionAssert.IsNotEmpty(errors);

			Assert.AreEqual("StepValue1 is required", errors.Single());
			Assert.AreEqual(errors.Single(),Sut.FieldValidatorValue);
		}
		[Test]
		public async Task AndInputIsNumeric_Next_ShowsError()
		{
			Sut.Input.Value = "22";
			Sut = (await Sut.ClickOnElementByText("Next")).CurrentPageAs<GreenFlowStep0>();
			var errors = Sut.Errors();
			CollectionAssert.IsNotEmpty(errors);
			Assert.AreEqual(string.Empty, Sut.FieldValidatorValue);
			Assert.AreEqual("Numeric only value  are not allowed",errors.Single());
		}

		[Test]
		public async Task AndInputIsA_Next_ShowsStepC_ThenPreviousKeptInput()
		{
			Sut.Input.Value = "a";
			Sut = (await (await Sut.ClickOnElementByText("Next"))
					.CurrentPageAs<GreenFlowStepC>()
					.ClickOnElementByText("Previous"))
				.CurrentPageAs<GreenFlowStep0>();

			Assert.AreEqual("a",Sut.Input.Value);
			var errors = Sut.Errors();
			Assert.IsTrue(!errors.Any());
			Assert.AreEqual(string.Empty, Sut.FieldValidatorValue);
		}

		[Test]
		public async Task AndInputIsAnother_Next_ShowsStepA_ThenPreviousKeptInput()
		{
			Sut.Input.Value = "sdfa";
			Sut = (await (await Sut.ClickOnElementByText("Next"))
					.CurrentPageAs<GreenFlowStepA>()
					.ClickOnElementByText("Previous"))
				.CurrentPageAs<GreenFlowStep0>();

			Assert.AreEqual("sdfa", Sut.Input.Value);
			var errors = Sut.Errors();
			Assert.IsTrue(!errors.Any());
			Assert.AreEqual(string.Empty, Sut.FieldValidatorValue);
		}
		
		[TestCase("22")]
		[TestCase("sadf")]
		public async Task Resets_RestartsStep0(string input)
		{
			if (input != null)
			{
				Sut.Input.Value = input;
			}
			Sut = (await Sut.ClickOnElementByText("Reset")).CurrentPageAs<GreenFlowStep0>();
			CollectionAssert.IsEmpty(Sut.Errors());
			Assert.IsEmpty(Sut.Input.Value);
			Assert.AreEqual(string.Empty, Sut.FieldValidatorValue);
		}


	}
}