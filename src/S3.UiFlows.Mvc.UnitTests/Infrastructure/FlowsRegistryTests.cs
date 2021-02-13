using System;
using System.Collections.Generic;
using NUnit.Framework;
using S3.App.Flows.AppFlows.BlueFlow.FlowDefinitions;
using S3.App.Flows.AppFlows.BlueFlow.Steps;
using S3.App.Flows.AppFlows.BlueFlow.Steps.FillData;
using S3.App.Flows.AppFlows.ComponentsFlow.Components.SampleComponentAsync;
using S3.App.Flows.AppFlows.ContainersFlow4.Steps;
using S3.App.Flows.SharedFlowComponents.Main.SampleInput;
using S3.UiFlows.Mvc.Infrastructure;
using FlowInitializer = S3.App.Flows.AppFlows.ComponentsFlow.Steps.FlowInitializer;

namespace S3.UiFlows.Mvc.UnitTests.Infrastructure
{
	class FlowsRegistryTests
	{

		public static IEnumerable<TestCaseData> CanResolveFlowByType_Cases()
		{
			var blueflow = "BlueFlow".ToLowerInvariant();
			yield return new TestCaseData(typeof(IBlueInput)).Returns(blueflow);
			yield return new TestCaseData(typeof(InitialScreen)).Returns(blueflow);
			yield return new TestCaseData(typeof(StepAScreen)).Returns(blueflow);


			var componentsflow = "ComponentsFlow".ToLowerInvariant();
			yield return new TestCaseData(typeof(SampleComponentAsync)).Returns(componentsflow);
			yield return new TestCaseData(typeof(SampleComponentInputAsync)).Returns(componentsflow);
			yield return new TestCaseData(typeof(SampleComponentViewModelAsync)).Returns(componentsflow);

			yield return new TestCaseData(typeof(FlowInitializer)).Returns(componentsflow);
			yield return new TestCaseData(typeof(Number1ContainerScreen)).Returns("ContainersFlow4".ToLowerInvariant());


			yield return new TestCaseData(typeof(InputSampleStepComponent)).Returns(null);
		}

		[TestCaseSource(nameof(CanResolveFlowByType_Cases))]
		public string CanResolveFlowByType(Type type)
		{
			var sut = new FlowsRegistry(typeof(InitialScreen).Assembly, "S3.App.Flows.AppFlows", "/Flows/AppFlows");

			return sut.GetByType(type,false)?.Name;
		}
	}
}
