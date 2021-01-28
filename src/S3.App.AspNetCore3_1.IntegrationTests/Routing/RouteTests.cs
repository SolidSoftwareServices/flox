using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using S3.CoreServices.Serialization;
using S3.CoreServices.System;


using S3.App.AspNetCore3_1.Flows.AppFlows;
using S3.App.AspNetCore3_1.IntegrationTests.Routing.Misc;
using S3.UI.TestServices.Test;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Mvc;
using S3.UiFlows.Mvc.Controllers;
using NUnit.Framework;

namespace S3.App.AspNetCore3_1.IntegrationTests.Routing
{
	[TestFixture]
	class RouteTests : WebAppTestsBase<RawApp<TestStartUp>>
	{
		
		public RouteTests() : base(()=>new RawApp<TestStartUp>())
		{
		}

		[Test]
		public async Task CanResolve_UiFlow_Init_WhenNotContained()
		{
			var uri = $"/{nameof(SampleAppFlowType.BlueFlow)}/{nameof(UiFlowController.Init)}";
			string containerId = null;

			await Execute_Init(uri, containerId);
		}
		[Test]
		public async Task CanResolve_UiFlow_Init_When_Contained()
		{

			string containerId = Guid.NewGuid().ToString();
			var uri = $"/{nameof(SampleAppFlowType.BlueFlow)}/{nameof(UiFlowController.Init)}/{containerId}";
			await Execute_Init(uri, containerId);
		}

		private async Task Execute_Init(string uri, string assertContainerId)
		{
			await _ExecuteGet<UiFlowController.InitializeUiFlowRequest>(uri, nameof(IUiFlowController.Init),
				(actualRequested) =>
				{
					Assert.AreEqual(SampleAppFlowType.BlueFlow, actualRequested.FlowType.ToEnum<SampleAppFlowType>());
					if (assertContainerId == null)
					{
						Assert.IsNull(actualRequested.ContainerFlowHandler);
					}
					else
					{
						Assert.AreEqual(assertContainerId, actualRequested.ContainerFlowHandler);
					}
				});
		}


		[Test]
		public async Task CanResolve_GetCurrentContainedView()
		{
			string flowHandler = Guid.NewGuid().ToString();
			var uri = $"/{nameof(SampleAppFlowType.BlueFlow)}/{nameof(UiFlowController.ContainedView )}?ContainedFlowHandler={flowHandler}";
			await _ExecuteGet<UiFlowController.ContainedViewRequest>(uri,
				nameof(IUiFlowController.ContainedView),
				(actualRequested) => { Assert.AreEqual(flowHandler, actualRequested.ContainedFlowHandler); });
		}

		[Test]
		public async Task CanResolve_GetNewContainedView()
		{
			string flowHandler = Guid.NewGuid().ToString();
			var uri = $"/{nameof(SampleAppFlowType.BlueFlow)}/{nameof(UiFlowController.NewContainedView)}?FlowHandler={flowHandler}&NewContainedFlowType=greenflow";
			await _ExecuteGet<UiFlowController.GetNewContainedViewRequest>(uri,
				nameof(IUiFlowController.NewContainedView),
				(actualRequested) =>
				{
					Assert.IsNotNull(actualRequested.FlowHandler);
					Assert.AreEqual(flowHandler, actualRequested.FlowHandler);

					Assert.IsNotNull(actualRequested.NewContainedFlowType);
					Assert.AreEqual(SampleAppFlowType.GreenFlow, actualRequested.NewContainedFlowType.ToEnum<SampleAppFlowType>());
				});
		}

		[Test]
		public async Task CanResolve_UiFlow_GetCurrentView()  
		{
			string flowHandler = Guid.NewGuid().ToString();
			var uri = $"/{nameof(SampleAppFlowType.BlueFlow)}/{nameof(UiFlowController.Current)}?FlowHandler={flowHandler}";
			await _ExecuteGet<UiFlowController.CurrentViewRequest>(uri,
				nameof(IUiFlowController.Current),
				(actualRequested) =>
				{
					Assert.IsNotNull(actualRequested.FlowHandler);
					Assert.AreEqual(flowHandler, actualRequested.FlowHandler);
				});
		}



		[Test]
		public async Task CanResolve_UiFlow_OnEvent()
		{
			string trigger = Guid.NewGuid().ToString();
			var uri = $"/{nameof(SampleAppFlowType.BlueFlow)}/{nameof(UiFlowController.OnEvent)}";

			var step = new Fixture().Build<UiFlowScreenModel>()
				.With(x=>x.FlowScreenName,typeof(UiFlowScreenModel).FullName)
				.Create();
			
			var bodyData =(IDictionary<string,object>) step.ToDynamic();
			bodyData.Add(SharedSymbol.FlowEventFormFieldName, trigger);
			var msg = new HttpRequestMessage(HttpMethod.Post, uri);
            var propertiesToIgnore = new[]
            {
                nameof(UiFlowScreenModel.Metadata)
            };
            msg.Content = new FormUrlEncodedContent(bodyData.Where(x => !propertiesToIgnore.Contains(x.Key) && x.Value != null)
				.Select(x => new KeyValuePair<string, string>(x.Key, x.Value.ToString())));

			var response = await App.Client.Value.SendAsync(msg);
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

			var body = await response.Content.ReadAsStringAsync();

			var actual = body.JsonToObject<UiFlowRoutingResult>();

			Assert.AreEqual(uri, actual.RequestedUrl);
			Assert.AreEqual(typeof(RoutingUiFlowController).Name, actual.Controller);
			Assert.AreEqual(nameof(UiFlowController.OnEvent), actual.Action);

			Assert.AreEqual(trigger, actual.RequestArgs[0]);

			var actualData = actual.RequestArgs[1].JsonToObject<UiFlowScreenModel>();
			Assert.AreEqual(step, actualData);
		}



		async Task _ExecuteGet<TRequestModel>(string getUri, string expectedActionHandler, Action<TRequestModel> assertAction)
		{
			var response = await App.Client.Value.GetAsync(getUri);
			Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

			var body = await response.Content.ReadAsStringAsync();

			var actual = body.JsonToObject<UiFlowRoutingResult>();

			Assert.AreEqual(getUri, actual.RequestedUrl);
			Assert.AreEqual(typeof(RoutingUiFlowController).Name, actual.Controller);
			Assert.AreEqual(expectedActionHandler, actual.Action);

			var actualRequested = actual.RequestArgs.Single().JsonToObject<TRequestModel>();
			assertAction(actualRequested);
		}

	}
}