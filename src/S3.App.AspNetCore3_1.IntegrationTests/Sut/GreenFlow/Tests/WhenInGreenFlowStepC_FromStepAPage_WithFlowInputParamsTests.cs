using NUnit.Framework;
using S3.App.Flows.AppFlows.GreenFlow.Steps;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.GreenFlow.Tests
{
	[TestFixture]
	class WhenInGreenFlowStepC_FromStepAPage_WithFlowInputParamsTests:WhenInGreenFlowStepC_FromStepAPageTests
	{
		public override (string, string)[] InputQueryStringParameters { get; } = {(nameof(FlowInitializer.StartScreenModel.SampleParameter), "1asdfasd")};
	}
}