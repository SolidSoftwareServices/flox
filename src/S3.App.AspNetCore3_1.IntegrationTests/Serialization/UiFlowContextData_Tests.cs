using System.Threading.Tasks;
using S3.CoreServices.Serialization;
using S3.App.AspNetCore3_1.Flows.AppFlows;
using S3.UiFlows.Core.Flows.Screens.Models.Interop;

using NUnit.Framework;

namespace S3.App.AspNetCore3_1.IntegrationTests.Serialization
{
	[Ignore("Use only when needed")]
	[TestFixture]
	class UiFlowContextData_Tests
	{
		
		[Test]
		public async Task CanDeserialize()
		{
			var src = "{\r\n      \r\n      \"StartFlowType\": \"BlueFlow\",\r\n      \"StartData\": {\r\n        \"$type\": \"<>f__AnonymousType9`1[[System.String, System.Private.CoreLib]], S3.App.AspNetCore3_1\",\r\n        \"GreenFlowData\": \"aas\"\r\n      },\r\n      \"FlowHandler\": \"4c7a2fdd-a51c-46f7-ae3b-77535301e9c9\",\r\n      \"FlowScreenName\": \"S3.UiFlows.Core.Steps.StepModels.ConnectToFlow`1[[S3.App.AspNetCore3_1.Flows.AppFlows.SampleAppFlowType, S3.App.AspNetCore3_1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]\",\r\n      \"DateCreated\": \"2019-12-22T22:42:21.7063113Z\",\r\n      \"ContainerProperties\": {},\r\n      \"Errors\": [],\r\n      \"DontValidateEvents\": {\r\n        \"$type\": \"S3.UiFlows.Core.Flows.ScreenEvent[], S3.UiFlows.Core\",\r\n        \"$values\": [\r\n          {\r\n            \"ValueId\": \"ErrorOccurred\",\r\n            \"DisplayValue\": \"ErrorOccurred\"\r\n          },\r\n          {\r\n            \"ValueId\": \"Start\",\r\n            \"DisplayValue\": \"Start\"\r\n          }\r\n        ]\r\n      }\r\n    }";

			var ctx= src.JsonToObject<ConnectToFlow>(true);
		}

		

	}
}