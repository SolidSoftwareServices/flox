using EI.RP.NavigationPrototype.Flows.AppFlows.GreenFlow.Steps;
using NUnit.Framework;

namespace EI.RP.NavigationPrototype.IntegrationTests.Sut.GreenFlow.Tests
{
	[TestFixture]
	class WhenInGreenFlowStepC_FromStepAPage_WithFlowInputParamsTests:WhenInGreenFlowStepC_FromStepAPageTests
	{
		public override (string, string)[] InputQueryStringParameters { get; } = {(nameof(FlowInitializer.StartScreenModel.SampleParameter), "1asdfasd")};
	}
}