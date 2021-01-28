using EI.RP.NavigationPrototype.Flows.AppFlows.BlueFlow.Steps;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.BlueFlow.Tests
{
	[TestFixture]
	class WhenInBlueFlowStepC_FromStepAPage_WithFlowInputParamsTests:WhenInBlueFlowStepC_FromStepAPageTests
	{
		public override (string, string)[] InputQueryStringParameters { get; } = { (nameof(FlowInitializer.StartScreenModel.GreenFlowData), "adsfasdf") };
	}
}