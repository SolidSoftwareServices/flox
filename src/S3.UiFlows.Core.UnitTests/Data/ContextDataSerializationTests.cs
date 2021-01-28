using System;
using S3.CoreServices.Serialization;
using S3.UiFlows.Core.Infrastructure.DataSources;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Screens;
using NUnit.Framework;

namespace S3.UiFlows.Core.UnitTests.Data
{
	[TestFixture]
	class ContextDataSerializationTests 
	{
		[Test]
		public void CanDeserialize()
		{
			var ctx = new UiFlowContextData();
			ctx.CurrentScreenStep = ScreenName.PreStart;
			var flowScreenName = "stepDataA";
			var expected = ctx.AddStepData(flowScreenName);
			var actual =ctx.ToJson(true).JsonToObject<UiFlowContextData>(true);
			var userMetadata = actual.ViewModels[flowScreenName];
			Assert.AreEqual(ctx.FlowHandler, userMetadata.FlowHandler);
			Assert.IsNotNull(userMetadata.UserData);
			Assert.AreEqual(expected, userMetadata.UserData);

		}

		
	}
}