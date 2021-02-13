using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using S3.CoreServices.Serialization;
using S3.CoreServices.System;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Mvc;
using S3.UiFlows.Mvc.Controllers;

namespace S3.App.AspNetCore3_1.IntegrationTests.Routing.Misc
{
	public class RoutingUiFlowController:Controller,IUiFlowController
	{
		[HttpGet()]
		[HttpGet("{ContainerFlowHandler}")]
		public async Task<IActionResult> Init(UiFlowController.InitializeUiFlowRequest request)
		{
			//TODO: REMOVE AFTER REFACTOR
			request.FlowType = Request.Path.ToString().Split('/').First(x => !string.IsNullOrWhiteSpace(x));
			var result = BuildResult(request.ToOneItemArray());
			
			return Ok(result);
		}

	
		public async Task<IActionResult> Current(UiFlowController.CurrentViewRequest request)
		{
				var result = BuildResult(request.ToOneItemArray());
			return Ok(result);
		}

		public async Task<IActionResult> ContainedView(UiFlowController.ContainedViewRequest request)
		{
			var result = BuildResult(request.ToOneItemArray());
			return Ok(result);
		}

		public async Task<IActionResult> NewContainedView(UiFlowController.GetNewContainedViewRequest request)
		{
			var result = BuildResult(request.ToOneItemArray());
			return Ok(result);
		}
		[HttpPost]
		public async Task<IActionResult> OnEvent([FromForm(Name = SharedSymbol.FlowEventFormFieldName)]string trigger,[FromForm] UiFlowScreenModel model)
		{
			if (trigger == null) throw new ArgumentNullException(nameof(trigger));
			var result = BuildResult(new object[]{trigger,model});
			return Ok(result);
		}

		private UiFlowRoutingResult BuildResult(object[] args,[CallerMemberName] string memberName = "")
		{
			var result = new UiFlowRoutingResult();
			result.Controller = GetType().Name;
			result.Action = memberName;
			result.RequestedUrl = Request.Path + Request.QueryString;
			var enumerable = args.Select(x=>(x.GetType().IsValueType || x is string)?x.ToString(): x.ToJson());
			result.RequestArgs = enumerable.ToArray();
			//TODO: REMOVE AFTER REFACTOR
			result.FlowType = Request.Path.ToString().Split('/').First(x => !string.IsNullOrWhiteSpace(x));
			return result;
		}

	}
}