using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using AutoFixture;
using S3.CoreServices.System;
using S3.UI.TestServices.Flows;
using S3.UI.TestServices.Flows.FlowInitializerUnitTest;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Screens;
using NUnit.Framework;
using S3.App.Flows.AppFlows;
using S3.App.Flows.AppFlows.GreenFlow.FlowDefinitions;
using S3.App.Flows.AppFlows.GreenFlow.Steps;

namespace S3.App.AspNetCore3_1.UnitTests.Flows.AppFlows.GreenFlow
{
	[TestFixture]
	public class GreenFlowInitializerTests
	{
		private readonly IFixture _fixture = new Fixture();
		
		[Test, Theory]
		public void InitializationIsCorrect(bool withInputParameters)
		{
			var configurator = new FlowInitializerTestConfigurator<FlowInitializer>(_fixture);
			ExpandoObject p = null;
			if (withInputParameters)
			{
				p = new
				{
					sampleParameter = _fixture.Create<string>(),
					Another = _fixture.Create<string>()
				}.ToExpandoObject();
			}
			configurator.NewInitializationRunner().WithInput(withInputParameters ? p : null)
				.Run()
				.AssertStepData<FlowInitializer.StartScreenModel>(stepData =>
				{
					Assert.AreEqual(withInputParameters ? p.ToDynamic().sampleParameter : null,
						stepData.SampleParameter);
				})
				.AssertTriggeredEventIs((actual)=>Assert.AreEqual(ScreenEvent.Start,actual));
		}

		[Test]
		public  void FlowTypeIsCorrect()
		{
			var configurator = new FlowInitializerTestConfigurator<FlowInitializer>(_fixture);
			var adapter = configurator.Adapter;
			Assert.AreEqual("GreenFlow".ToLowerInvariant(),adapter.GetFlowType() );

		}

		[Test]
		public  async Task CanAuthorize()
		{
			var configurator = new FlowInitializerTestConfigurator<FlowInitializer>(_fixture);
			var adapter = configurator.Adapter;
			await adapter.Initialize();
			Assert.IsTrue(adapter.Authorize());
		}

		public static IEnumerable<TestCaseData> HandlingIsCorrect_OnStartCases()
		{
			yield return new TestCaseData(new FlowInitializer.StartScreenModel(), GreenFlowScreenName.Step0Screen).SetName($"{nameof(HandlingIsCorrect_OnStartCases)}_1");
			yield return new TestCaseData(new FlowInitializer.StartScreenModel
			{
				SampleParameter = "asdf"
			}, GreenFlowScreenName.Step0Screen).SetName($"{nameof(HandlingIsCorrect_OnStartCases)}_2");

			yield return new TestCaseData(new FlowInitializer.StartScreenModel
			{
				SampleParameter = "Finish"
			}, GreenFlowScreenName.FlowCompletedScreen).SetName($"{nameof(HandlingIsCorrect_OnStartCases)}_3");
		}



		[TestCaseSource(nameof(HandlingIsCorrect_OnStartCases))]
		public void HandlingIsCorrect_OnStart(FlowInitializer.StartScreenModel whenDataIs, ScreenName expectedStep)
		{
			var configurator = new FlowInitializerTestConfigurator<FlowInitializer>(_fixture);
			configurator.NewEventTestRunner()
				.GivenTheStepDataIs(whenDataIs)
				.WhenEvent(ScreenEvent.Start)
				.TheResultStepIs((actualStep) => Assert.AreEqual(expectedStep, actualStep));

		}
	}
}