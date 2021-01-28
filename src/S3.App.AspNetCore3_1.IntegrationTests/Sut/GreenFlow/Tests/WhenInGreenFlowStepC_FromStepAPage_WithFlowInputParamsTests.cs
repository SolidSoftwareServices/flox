using S3.App.AspNetCore3_1.Flows.AppFlows.GreenFlow.Steps;
using NUnit.Framework;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.GreenFlow.Tests
{
	[TestFixture]
	class WhenInGreenFlowStepC_FromStepAPage_WithFlowInputParamsTests:WhenInGreenFlowStepC_FromStepAPageTests
	{
		public override (string, string)[] InputQueryStringParameters { get; } = {(nameof(FlowInitializer.StartScreenModel.SampleParameter), "1asdfasd")};
	}
}