using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using S3.CoreServices.Profiling;
using S3.CoreServices.Serialization;
using S3.CoreServices.System;
using S3.Mvc.Core.Authx;
using S3.Mvc.Core.Controllers;
using S3.Mvc.Core.Cryptography.AntiTampering;
using S3.Mvc.Core.Cryptography.Urls;
using S3.Mvc.Core.System;
using S3.Mvc.Core.System.Request;
using S3.UiFlows.Core.Facade;
using S3.UiFlows.Core.Facade.CurrentView;
using S3.UiFlows.Core.Facade.FlowResultResolver;
using S3.UiFlows.Core.Facade.InitialView;
using S3.UiFlows.Core.Facade.Metadata;
using S3.UiFlows.Core.Facade.SetContainedView;
using S3.UiFlows.Core.Facade.TriggerEventOnView;
using S3.UiFlows.Core.Flows;
using S3.UiFlows.Core.Flows.Screens.ErrorHandling;
using S3.UiFlows.Core.Flows.Screens.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Net.Http.Headers;
using NLog;
using S3.UiFlows.Core.DataSources;

namespace S3.UiFlows.Mvc.Controllers
{
	/// <summary>
	///     Uiflow controller
	/// </summary>

	[AutoValidateAntiforgeryToken]
	[SupportEncryptedUrls]
	[ValidateHiddenInputsAntiTampering]
	[DecryptHiddenInputs]
	public class UiFlowController :
		Controller,
		IUiFlowController
	{
		private const string LastProcessedTokenKey = "LastProcessedToken";
		public const string FlowFormKey = "flow-form-id";

		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IAppUiFlowsCollection _flows;
		private readonly IFlowInitialViewRequestHandler<IActionResult> _flowInitialViewResolver;
		private readonly IFlowCurrentViewRequestHandler<IActionResult> _flowCurrentViewResolver;
		private readonly IFlowTriggerEventOnViewHandler<IActionResult> _flowExecuteEvent;
		private readonly IFlowResultResolverRequestHandler<IActionResult> _resultResolver;
		private readonly ISetContainedViewRequestHandler<IActionResult> _setNewContainedViewHandler;
		private readonly IFlowsMetadataResolver _metadataResolver;
		private readonly IUiFlowContextRepository _contextRepository;
		private readonly IProfiler _profiler;
		private IFlowsStore _store;

		public UiFlowController(IUiFlowContextRepository contextRepository,
			IProfiler profiler, IFlowsStore store,
			IAppUiFlowsCollection flows
			, IFlowInitialViewRequestHandler<IActionResult> flowInitialViewResolver
			, IFlowCurrentViewRequestHandler<IActionResult> flowCurrentViewResolver,
			IFlowTriggerEventOnViewHandler<IActionResult> flowExecuteEvent
			, IFlowResultResolverRequestHandler<IActionResult> resultResolver,
			ISetContainedViewRequestHandler<IActionResult> setNewContainedViewHandler
			, IFlowsMetadataResolver metadataResolver)
		{

			_contextRepository = contextRepository;
			_profiler = profiler;
			_store = store;
			_flows = flows;
			_flowInitialViewResolver = flowInitialViewResolver;
			_flowCurrentViewResolver = flowCurrentViewResolver;
			_flowExecuteEvent = flowExecuteEvent;
			_resultResolver = resultResolver;
			_setNewContainedViewHandler = setNewContainedViewHandler;
			_metadataResolver = metadataResolver;
		}

		public class InitializeUiFlowRequest
		{
			//TODO: BIND THIS
			public string FlowType { get; set; }
			public string ContainerFlowHandler { get; set; }
			public string StartType { get; set; }

			public override string ToString()
			{
				return
					$"{nameof(FlowType)}: {FlowType}, {nameof(ContainerFlowHandler)}: {ContainerFlowHandler}, {nameof(StartType)}: {StartType}";
			}
		}

		[HttpGet]
		public async Task<IActionResult> Init(InitializeUiFlowRequest request)
		{
			using (_profiler.Profile(nameof(UiFlowController), nameof(Init)))
			{
				Logger.Trace(() => $"Starting - {nameof(Init)} --> {request} ");
				var result = await this.HandleActionAsync(async () =>
				{
					var input = ControllerContext.RouteData.Values.ToExpandoObject(Request.Query);
					return await _flowInitialViewResolver.Execute(
						new InitialViewRequest<IActionResult>
						{
							ContainerFlowHandler = request.ContainerFlowHandler,
							FlowInput = input.ToExpandoObject(),
							FlowType = ResolveFlowTypeFromValueOrUrl(request.FlowType),
							StartType = request.StartType,
							OnBuildView = async model =>
							{
								var resolverRequest = new FlowResultResolverRequest<IActionResult>
								{
									ScreenModel = model
								};
								AddDefaultCallbackHandlers(resolverRequest);
								return await _resultResolver.Execute(resolverRequest);
							}
						});
				});

				Logger.Trace(() => $"Completed - {nameof(Init)} --> {request} ");
				return result;
			}
		}



		public class CurrentViewRequest
		{
			public string FlowHandler { get; set; }

			public override string ToString()
			{
				return $"{nameof(FlowHandler)}: {FlowHandler}";
			}
		}

		[HttpGet]
		public async Task<IActionResult> Current(CurrentViewRequest request)
		{
			using (_profiler.Profile(nameof(UiFlowController), nameof(Current)))
			{
				Logger.Trace(() => $"Starting - {nameof(Current)} --> {request} ");
				var result = await this.HandleActionAsync(async () =>
				{
					var stepViewCustomizations =
						(IDictionary<string, object>) ControllerContext.RouteData.Values.ToExpandoObject(Request.Query);
					return await _flowCurrentViewResolver.Execute(
						new CurrentViewRequest<IActionResult>
						{
							FlowHandler = request.FlowHandler,
							ViewParameters = stepViewCustomizations,
							BuildResultOnFlowNotFound = () => Task.FromResult((IActionResult) BadRequest()),
							BuildResultOnRequestRedirectTo = async (flowHandler)
								=> (IActionResult) RedirectToAction(nameof(Current),
									GetFlowTypeFromUrl(), await stepViewCustomizations.MergeObjects(preserveLast: true,
										new CurrentViewRequest
										{
											FlowHandler = flowHandler
										}).ToValidRouteValueAsync(HttpContext)),
							OnAddModelError = (memberName, errorMessage) =>
								ModelState.AddModelError(memberName, errorMessage),
							OnBuildView = (bvi) =>
							{
								var viewPath = $"{bvi.FlowType}/Views/{bvi.ViewName}";
								return Task.FromResult(bvi.BuildPartial
									? PartialView(viewPath, bvi.ScreenModel)
									: (IActionResult) View(viewPath, bvi.ScreenModel));
							},
							OnCallbackCallerFlow =  async model =>
							{
								var resolverRequest = new FlowResultResolverRequest<IActionResult>
								{
									ScreenModel = model
								};
								AddDefaultCallbackHandlers(resolverRequest);
								return await _resultResolver.Execute(resolverRequest);
							},

							ResolveAsComponentOnly = false
						});

				});

				Logger.Trace(() => $"Completed - {nameof(Current)} --> {request} ");
				return result;
			}


		}



		public class ContainedViewRequest
		{
			public string ContainedFlowHandler { get; set; }
			public string StartType { get; set; }

			public override string ToString()
			{
				return $"{nameof(ContainedFlowHandler)}: {ContainedFlowHandler}, {nameof(StartType)}: {StartType}";
			}
		}

		[HttpGet]
		public async Task<IActionResult> ContainedView(ContainedViewRequest request)
		{
			using (_profiler.Profile(nameof(UiFlowController), nameof(ContainedView)))
			{
				Logger.Trace(() => $"Starting - {nameof(ContainedView)} --> {request} ");
				if (string.IsNullOrEmpty(request.ContainedFlowHandler))
				{
					//for the exception handler to redirect to error
					throw new InvalidOperationException();
				}

				var actionResult = await Current(new CurrentViewRequest {FlowHandler = request.ContainedFlowHandler});
				Logger.Trace(() => $"Completed - {nameof(ContainedView)} --> {request} ");
				return actionResult;
			}
		}

		public class GetNewContainedViewRequest
		{
			public string FlowHandler { get; set; }
			public string NewContainedFlowType { get; set; }
			public string StartType { get; set; }
		}

		//TODO: THIS SHOULD BE THE COMMAND CONCERN AND ENCAPSULATED IN IT
		private async Task<IActionResult> RecordAndResolve(UiFlowScreenModel sourceStep)
		{
			IActionResult result = null;
			var form = HttpContext.Request.Form;
			if (!form.ContainsKey(FlowFormKey))
			{
				result = BadRequest();
			}
			else
			{
				var currentToken = form[FlowFormKey].ToString();

				var lastToken = await _store.GetAsync(LastProcessedTokenKey);
				if (lastToken == currentToken)
				{
					var ctx = await _contextRepository.GetCurrentSnapshot(sourceStep.FlowHandler);
					var uiFlowStepData = ctx.GetCurrentStepData<UiFlowScreenModel>();
					while (uiFlowStepData == null)
					{
						await Task.Delay(100);
						ctx = await _contextRepository.GetCurrentSnapshot(sourceStep.FlowHandler);
						uiFlowStepData = ctx.GetCurrentStepData<UiFlowScreenModel>();
					}

					var request = new FlowResultResolverRequest<IActionResult>
					{
						ScreenModel = uiFlowStepData
					};
					AddDefaultCallbackHandlers(request);
					result = await _resultResolver.Execute(request);

				}

				await _store.SetAsync(LastProcessedTokenKey, currentToken);
			}

			return result;
		}


		[HttpPost]
		public async Task<IActionResult> OnEvent([FromForm(Name = SharedSymbol.FlowEventFormFieldName)]
			string trigger, [FromForm] UiFlowScreenModel model)
		{

			using (_profiler.Profile(nameof(UiFlowController), nameof(OnEvent)))
			{
				IActionResult result;
				Logger.Trace(() => $"Starting - {nameof(OnEvent)} --> trigger:{trigger} model:{model} ");
				if (model == null)
				{
					return BadRequest("model not specified");
				}

				try
				{
					var alreadyProcessedResult = await RecordAndResolve(model);
					if (alreadyProcessedResult != null)
					{
						return alreadyProcessedResult;
					}
				}
				catch (Exception ex)
				{
					Logger.Warn(()=>ex.ToString());
				}

				result = await this.HandleActionAsync(async () =>
				{
					ModelState.Clear();

					var triggerEventOnView = new TriggerEventOnView<IActionResult>
					{
						Event = trigger,
						Model = model,

						OnExecuteValidation = ValidateStepModel

					};
					AddDefaultCallbackHandlers(triggerEventOnView);
					return await _flowExecuteEvent.Execute(triggerEventOnView);
				});

				Logger.Trace(() => $"Completed - {nameof(OnEvent)} --> trigger:{trigger} model:{model} ");
				return result;

			}

			IEnumerable<UiFlowUserInputError> ValidateStepModel(UiFlowScreenModel stepModel)
			{
				ModelState.Clear();
				TryValidateModel(stepModel);
				return GetValidationErrorsAsFlowErrors();

				IEnumerable<UiFlowUserInputError> GetValidationErrorsAsFlowErrors()
				{
					var errors = new List<UiFlowUserInputError>();
					foreach (var itemName in ModelState.Where(x =>
						x.Value.ValidationState == ModelValidationState.Invalid))
					{
						var itemNameKey = itemName.Key;
						errors.AddRange(itemName.Value.Errors
							.Where(x => !errors.Any(y =>
								y.MemberName == itemNameKey && x.ErrorMessage == y.ErrorMessage)).Select(x =>
								new UiFlowUserInputError
									{ErrorMessage = x.ErrorMessage, MemberName = itemNameKey}));
					}

					return errors;
				}

			}


		}

		private void AddDefaultCallbackHandlers(ICallbackHandler<IActionResult> callbackHandlers)
		{
			callbackHandlers.OnExecuteEvent = async (eventName, screenModel) =>
			{
				await _store.RemoveAsync(LastProcessedTokenKey);
				return await OnEvent(eventName, screenModel);
			};
			callbackHandlers.OnExecuteRedirection = (redirectTo) =>
				Task.FromResult((IActionResult) RedirectToAction(redirectTo.ActionName,
					redirectTo.ControllerName));
			callbackHandlers.OnExecuteUnauthorized =
				unauthorizedModel => Task.FromResult((IActionResult) Unauthorized());
			callbackHandlers.OnRedirectToRoot = async(type, handler) => RedirectToAction(nameof(Current), type,
				await new CurrentViewRequest {FlowHandler = handler}.ToValidRouteValueAsync(ControllerContext.HttpContext));
			callbackHandlers.OnRedirectToCurrent = async (type, handler) => RedirectToAction(nameof(Current), type,
				(object) await  Request.Query.ToExpandoObject().MergeObjects(new {FlowHandler = handler})
					.ToEncryptedRouteValueAsync(ControllerContext.HttpContext));
			callbackHandlers.OnNewContainedScreen = (containerHandler, containerType, flowType, startInfo) =>
			{
				var requestData = startInfo.MergeObjects(new GetNewContainedViewRequest
				{
					FlowHandler = containerHandler,
					NewContainedFlowType = flowType,
				});
				return RedirectToAction(nameof(NewContainedView), containerType, requestData);
			};
			callbackHandlers.OnStartNewFlow =
				async(flowType, startInfo) => RedirectToAction(nameof(Init), flowType, await startInfo.ToValidRouteValueAsync(HttpContext));
			callbackHandlers.OnAddModelError =
				(memberName, errorMessage) => ModelState.AddModelError(memberName, errorMessage);
		}


		[HttpGet]
		public async Task<IActionResult> NewContainedView(GetNewContainedViewRequest request)
		{
			using (_profiler.Profile(nameof(UiFlowController), nameof(NewContainedView)))
			{
				Logger.Trace(() => $"Starting - {nameof(NewContainedView)} --> {request} ");
				var result = await this.HandleActionAsync(async () => await _GetNewContainedView(request));

				Logger.Trace(() => $"Completed - {nameof(NewContainedView)} --> {request} ");
				return result;
			}
		}

		private async Task<IActionResult> _GetNewContainedView(GetNewContainedViewRequest request)
		{

			var containedViewRequest = new SetContainedViewRequest<IActionResult>
			{
				FlowHandler = request.FlowHandler,
				ViewCustomizations = ControllerContext.RouteData.Values.ToExpandoObject(Request.Query),
				NewContainedFlowType = request.NewContainedFlowType,
				NewContainedFlowStartType = request.StartType
			};

			AddDefaultCallbackHandlers(containedViewRequest);


			containedViewRequest.OnRedirectToCurrent = async (type, handler) =>
			{
				var routeValues = await HttpContext.ToEncryptedRouteValuesAsync(
					new CurrentViewRequest
					{
						FlowHandler = handler
					}, containedViewRequest.ViewCustomizations.RemoveIfExists(nameof(GetNewContainedViewRequest.FlowHandler).ToLowerInvariant()));
				return RedirectToAction(nameof(Current), type,
					routeValues);
			};

			return await _setNewContainedViewHandler.Execute(containedViewRequest);
		}

		private string ResolveFlowTypeFromValueOrUrl(string flowType = null)
		{
			return (flowType ?? GetFlowTypeFromUrl()).ToLowerInvariant();
		}


		private string GetFlowTypeFromUrl()
		{
			return Request.Path.ToString().Split('/').First(x => !string.IsNullOrWhiteSpace(x));
		}

		[HttpGet]
		public async Task<IActionResult> DotGraph(string flowType = null)
		{
			Logger.Trace(() => $"Starting - {nameof(DotGraph)} --> flowType:{flowType}");
			var result = await this.HandleActionAsync(async () =>
			{
				var st = LoadScript();
				var dotGraphTask = _flows.GetByFlowType(ResolveFlowTypeFromValueOrUrl(flowType)).ToDotGraph();

				return new ContentResult
				{
					ContentType = "text/html",
					Content = BuildPage(await st, dotGraphTask)
				};
			});
			Logger.Trace(() => $"Completed - {nameof(DotGraph)} --> flowType:{flowType}");
			return result;


			string BuildPage(string script, string dotGraphDefinition)
			{
				var sb = new StringBuilder();
				sb.AppendLine("<html>");
				sb.AppendLine("<div id=\"svg_target\">\r\n  <!-- Target for dynamic svg generation -->\r\n</div>");

				sb.AppendLine(
					"<script language=\"javascript\" src=\"https://ajax.googleapis.com/ajax/libs/jquery/1.9.1/jquery.min.js\" type=\"text/javascript\"> </script>");
				var graphDefinition = HttpUtility.JavaScriptStringEncode(dotGraphDefinition);
				sb.AppendLine(
					$"<script language=\"javascript\">{script.Replace("[[DOT_GRAPH]]", graphDefinition)}</script>");
				sb.AppendLine("</html>");
				return sb.ToString();
			}

			async Task<string> LoadScript()
			{
				var type = GetType();
				using (var stream = type.Assembly.GetManifestResourceStream(type, "dotgraph.js"))
				{
					using (var textReader = new StreamReader(stream))
					{
						return await textReader.ReadToEndAsync();
					}
				}
			}
		}
		
		[AuthorizeOnlyDevelopers]
		[HttpGet]
		public async Task<IActionResult> Metadata()
		{
			Logger.Trace(() => $"Starting - {nameof(Metadata)}");
			var result=await this.HandleActionAsync(async () =>
			{
				var metadata = await _metadataResolver.GetMetadata();

				return Content(metadata.ToJson(true), MediaTypeHeaderValue.Parse("application/json"));
			});
			Logger.Trace(() => $"Completed - {nameof(Metadata)}");
			return result;
		}
	}
}