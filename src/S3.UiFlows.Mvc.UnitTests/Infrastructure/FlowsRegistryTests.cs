using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Internal;
using S3.App.Flows.AppFlows.BlueFlow.FlowDefinitions;
using S3.App.Flows.AppFlows.BlueFlow.Steps;
using S3.App.Flows.AppFlows.BlueFlow.Steps.FillData;
using S3.App.Flows.AppFlows.ComponentsFlow.Components.SampleComponentAsync;
using S3.App.Flows.AppFlows.ContainersFlow4.Steps;
using S3.App.Flows.SharedFlowComponents.Main.SampleInput;
using S3.UiFlows.Mvc.Infrastructure;

namespace S3.UiFlows.Mvc.UnitTests.Infrastructure
{
	class FlowsRegistryTests
	{

		public static IEnumerable<TestCaseData> CanResolveFlowByType_Cases()
		{
			yield return new TestCaseData(typeof(IBlueInput)).Returns("BlueFlow");
			yield return new TestCaseData(typeof(InitialScreen)).Returns("BlueFlow");
			yield return new TestCaseData(typeof(StepAScreen)).Returns("BlueFlow");


			yield return new TestCaseData(typeof(SampleComponentAsync)).Returns("ComponentsFlow");
			yield return new TestCaseData(typeof(SampleComponentInputAsync)).Returns("ComponentsFlow");
			yield return new TestCaseData(typeof(SampleComponentViewModelAsync)).Returns("ComponentsFlow");

			yield return new TestCaseData(typeof(S3.App.Flows.AppFlows.ComponentsFlow.Steps.FlowInitializer)).Returns("ComponentsFlow");
			yield return new TestCaseData(typeof(Number1ContainerScreen)).Returns("ContainersFlow4");


			yield return new TestCaseData(typeof(InputSampleStepComponent)).Returns(null);
		}

		[TestCaseSource(nameof(CanResolveFlowByType_Cases))]
		public string CanResolveFlowByType(Type type)
		{
			var sut = FlowsRegistry.Instance;
			sut.Load(typeof(InitialScreen).Assembly, "S3.App.Flows.AppFlows", "/Flows/AppFlows");

			return sut.GetByType(type,false)?.Name;
		}
	}
}
