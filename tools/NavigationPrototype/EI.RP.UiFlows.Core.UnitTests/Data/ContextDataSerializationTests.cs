using System;
using EI.RP.CoreServices.Serialization;
using EI.RP.UiFlows.Core.Infrastructure.DataSources;
using EI.RP.UiFlows.Core.Flows;
using EI.RP.UiFlows.Core.Flows.Screens;
using NUnit.Framework;

namespace EI.RP.UiFlows.Core.UnitTests.Data
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