using System;
using System.Collections.Generic;
using System.Text;
using S3.TestServices;
using S3.UiFlows.Core.Infrastructure.DataSources;
using NUnit.Framework;

namespace S3.UiFlows.Core.UnitTests.Data
{
	[TestFixture]
	class UiFlowContextDataTests : UnitTestFixture<UiFlowContextDataTests.TestContext, UiFlowContextData>
	{
		[Test]
		public void IsCreatedWithFlowHandler()
		{
			Assert.IsNotNull(Context.Sut.FlowHandler);
			Assert.IsNotEmpty(Context.Sut.FlowHandler);
		}

		[Test]
		public void FlowHandlerIsLowerInvariant()
		{
			Context.Sut.FlowHandler = "QWERTY";
			Assert.AreEqual("qwerty", Context.Sut.FlowHandler);
		}


		public class TestContext : UnitTestContext<UiFlowContextData>
		{

		}


	}
}
