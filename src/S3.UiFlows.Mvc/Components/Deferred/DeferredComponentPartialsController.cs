using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using S3.CoreServices.Profiling;
using S3.CoreServices.System;
using S3.CoreServices.System.FastReflection;
using S3.Mvc.Core.Cryptography.Urls;
using S3.Mvc.Core.System.Request;
using S3.UiFlows.Core.Flows.Screens.Models;
using Fasterflect;
using Microsoft.AspNetCore.Mvc;
using S3.UiFlows.Core.DataSources;

namespace S3.UiFlows.Mvc.Components.Deferred
{
	/// <summary>
	///     Uiflow set component async controller
	/// </summary>

	[AutoValidateAntiforgeryToken]
	[SupportEncryptedUrls]
	public class DeferredComponentPartialsController : Controller
	{
		private readonly IProfiler _profiler;
		private readonly IUiFlowContextRepository _flowRepository;

		public DeferredComponentPartialsController(IProfiler profiler,IUiFlowContextRepository flowRepository)
		{
			_profiler = profiler;
			_flowRepository = flowRepository;
		}

		
		[HttpGet]
		public async Task<IActionResult> Resolve()
		{
		
			using (_profiler.Profile(nameof(DeferredComponentPartialsController), nameof(Resolve)))
			{
				var input = (IDictionary<string, object>)ControllerContext.RouteData.Values.ToExpandoObject(Request.Query,forceLowerCaseOfValues:false);
				var handlerComponent = ResolveHandler(input);
				var getFlowContext = _flowRepository.Get(input[nameof(UiFlowContextData.FlowHandler).ToLowerInvariant()].ToString());
				var type = handlerComponent.GetType();
				var invocationInfo = type.GetFastMethodInvocationHandler("InvokeDeferredAsync");
				
				var componentInput = ((ExpandoObject) input).ToObjectOfType(invocationInfo.ArgumentTypes[1],false);
				var componentId = input[nameof(FlowComponentViewModel.ComponentId).ToLowerInvariant()].ToString();
				var screenModel=(await getFlowContext).GetCurrentStepData<UiFlowScreenModel>();

				var result = await invocationInfo.Execute<Task<IActionResult>>(handlerComponent,
					new object[]
						{componentId, componentInput, screenModel, (Func<string, object, IActionResult>) BuildView});
				return result;
			}

			IActionResult BuildView(string viewName,object model)
			{
				return PartialView(viewName, model);
			}
		}

		private object ResolveHandler(IDictionary<string, object> input)
		{
			var handler = TypesFinder.Resolver.FindType(input["handlertype"].ToString(), caseSensitive: false);
			var handlerComponent = HttpContext.RequestServices.GetService(handler);
			return handlerComponent;
		}
	}
}
