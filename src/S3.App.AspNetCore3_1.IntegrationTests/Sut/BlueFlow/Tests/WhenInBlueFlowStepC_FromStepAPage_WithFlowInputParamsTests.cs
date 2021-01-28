using S3.App.AspNetCore3_1.Flows.AppFlows.BlueFlow.Steps;
using NUnit.Framework;

namespace S3.App.AspNetCore3_1.IntegrationTests.Sut.BlueFlow.Tests
{
	[TestFixture]
	class WhenInBlueFlowStepC_FromStepAPage_WithFlowInputParamsTests:WhenInBlueFlowStepC_FromStepAPageTests
	{
		public override (string, string)[] InputQueryStringParameters { get; } = { (nameof(FlowInitializer.StartScreenModel.GreenFlowData), "adsfasdf") };
	}
}