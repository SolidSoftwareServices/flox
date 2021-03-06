using System.Collections.Generic;
using AutoFixture;
using NUnit.Framework;
using S3.App.Flows.AppFlows.GreenFlow.FlowDefinitions;
using S3.App.Flows.AppFlows.GreenFlow.Steps;
using S3.UI.TestServices.Flows.FlowScreenUnitTest;
using S3.UiFlows.Core.Flows.Screens;

namespace S3.App.AspNetCore3_1.UnitTests.Flows.AppFlows.GreenFlow
{
	[TestFixture]
	public class GreenFlowInitialScreenTests
	{
		private readonly IFixture _fixture = new Fixture();
		private FlowScreenTestConfigurator<InitialScreen> NewScreenTestConfigurator()
		{
			return new FlowScreenTestConfigurator<InitialScreen>(_fixture);
		}

		[Test]
		public void ViewPathIsCorrect()
		{
			Assert.AreEqual("Init", NewScreenTestConfigurator().Adapter.GetViewPath());
		}

		[Test]
		public void ScreenStepIsCorrect()
		{
			Assert.AreEqual(GreenFlowScreenName.Step0Screen, NewScreenTestConfigurator().Adapter.GetStep());
		}

		[Test]
		public void FlowIsCorrect()
		{
			Assert.AreEqual("GreenFlow".ToLowerInvariant(), NewScreenTestConfigurator().Adapter.GetFlowType());
		}

		[Test]
		public void CreatesStepDataCorrectly()
		{
			NewScreenTestConfigurator()
				.NewTestCreateStepDataRunner()
				.WithExistingStepData(ScreenName.PreStart, new FlowInitializer.StartScreenModel { SampleParameter = "aa" })
				.WhenCreated()
				.ThenTheStepDataIs<InitialScreen.InitialScreenScreenModel>(actual =>
				{
					Assert.IsNotNull(actual);
					Assert.IsNull(actual.StepValue1);
				});
		}

		[Test]
		public void RefreshesStepDataCorrectly()
		{
			var expected = _fixture.Create<string>();
			NewScreenTestConfigurator()
				.NewTestRefreshStepDataRunner()
				.WithExistingStepData(ScreenName.PreStart, new FlowInitializer.StartScreenModel { SampleParameter = "aa" })
				.GivenTheCurrentStepDataBeforeRefreshIs(new InitialScreen.InitialScreenScreenModel {StepValue1 = expected})
				.WhenRefreshed()
				.ThenTheStepDataIs<InitialScreen.InitialScreenScreenModel>(actual =>
				{
					Assert.IsNotNull(actual);
					Assert.AreEqual(expected,actual.StepValue1);
				});
		}


		public static IEnumerable<TestCaseData> HandlingIsCorrect_OnNextCases()
		{
			yield return new TestCaseData("aa",string.Empty).Returns(GreenFlowScreenName.StepCScreen);
			yield return new TestCaseData("bb", string.Empty).Returns(GreenFlowScreenName.StepAScreen);
			yield return new TestCaseData("111", "Numeric only value  are not allowed").Returns(GreenFlowScreenName.Step0Screen);
		}
		[TestCaseSource(nameof(HandlingIsCorrect_OnNextCases))]
		public ScreenName HandlingIsCorrect_OnNext(string inputText,string expectedValidationMessage)
		{
			return NewScreenTestConfigurator()
				.NewEventTestRunner(()=>new InitialScreen.InitialScreenScreenModel())
				.GivenTheStepDataIs(new InitialScreen.InitialScreenScreenModel() { StepValue1 = inputText })
				.WhenEvent(InitialScreen.ScreenInputEvent.Next)
				.ThenTheValidationResultIs(expectedValidationMessage== string.Empty, expectedValidationMessage)
				.ThenTheStepDataAfterIs<InitialScreen.InitialScreenScreenModel>(actual=>Assert.AreEqual(inputText,actual.StepValue1))
				.ResultStep;

		}

		[Test]
		public void HandlingIsCorrect_OnReset()
		{
			NewScreenTestConfigurator()
				.NewEventTestRunner(() => new InitialScreen.InitialScreenScreenModel())
				.WithExistingStepData(ScreenName.PreStart, new FlowInitializer.StartScreenModel
				{
					SampleParameter = "aa"
				})
				
				.WhenEvent(InitialScreen.ScreenInputEvent.Reset)
				.ThenTheStepDataAfterIsNull()
				.ThenTheResultStepIs(GreenFlowScreenName.Step0Screen)
				.ThenTheValidationWasNotExecuted();
		}
	}
}