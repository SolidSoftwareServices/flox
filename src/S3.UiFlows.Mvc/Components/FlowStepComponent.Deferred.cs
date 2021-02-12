using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using NLog;
using S3.CoreServices.EmbeddedResources;
using S3.CoreServices.Platform;
using S3.CoreServices.System;
using S3.CoreServices.System.FastReflection;
using S3.Mvc.Core.Controllers;
using S3.Mvc.Core.Cryptography.Urls;
using S3.Mvc.Core.System;
using S3.UiFlows.Core.Flows.Screens.Models;
using S3.UiFlows.Mvc.Components.Deferred;

namespace S3.UiFlows.Mvc.Components
{

	public abstract partial class FlowStepComponent<TComponentInput, TComponentModel>
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		protected IPlatformSettings PlatformSettings { get; }

		private static readonly ConcurrentDictionary<string, string> LoadingViewsCache =
			new ConcurrentDictionary<string, string>();

		/// <summary>
		/// indicates if the component will be rendered asynchronously after the container was rendered by the browser
		/// </summary>
		protected virtual bool DeferComponentLoad { get; } = false;
		protected virtual string ComponentId { get; private set; }
		
		
		private static KeyValuePair<string,string> Template { get; set; }= new KeyValuePair<string, string>("default-reserved-name",EmbeddedResourceReader.ReadTextResource("loader.cshtml"));
		private readonly Lazy<string> _loadingViewContent;
		protected FlowStepComponent(
			string viewName,
			IPlatformSettings platformSettings,
			string customLoadingViewName = null,
			[CallerFilePath] string componentPath = null)
			: this($"~{GetCustomViewPath(componentPath, viewName, "cshtml")}")
		{
			PlatformSettings = platformSettings;
			
			UpdateCachedLoaderView();
			SetComponentId();
			
			_loadingViewContent = new Lazy<string>(() =>
			{
				string customLoadingViewFullPath = null;
				if (customLoadingViewName != null)
				{
					customLoadingViewFullPath = GetCustomViewPath(componentPath, customLoadingViewName).TrimStart('~', '/');
				}

				return customLoadingViewFullPath != null
					? ResolveView(customLoadingViewFullPath)
					: string.Empty;
			});

			void SetComponentId()
			{
				var componentPrefix = ViewName.Replace(".cshtml", string.Empty, StringComparison.InvariantCultureIgnoreCase)
					.Replace('\\', '_')
					.Replace('/', '_')
					.Replace('~', '_')
					.Replace('.', '_')
					.TrimStart('_');
				ComponentId = $"{componentPrefix}_{Guid.NewGuid()}";
			}

			string ResolveView(string customLoadingViewFullPath)
			{
#if DEBUG || FRAMEWORKDEVELOPMENT
				//so it can be changed in the same debugging session
				return DoResolve();
#else
				return LoadingViewsCache.GetOrAdd(customLoadingViewFullPath, k =>DoResolve());
#endif
				string DoResolve()
				{
					var path = Path.Combine(HttpContext.Resolve<IWebHostEnvironment>().ContentRootPath,
						customLoadingViewFullPath);
					if (!File.Exists(path))
					{
						Logger.Error(() => $"Could not find loading view at :{path}");
						return string.Empty;
					}
					return File.ReadAllText(path).Replace("\"~/",$"\"{HttpContext.GetBaseUrl()}");
				}
			}


			void UpdateCachedLoaderView()
			{
				if (platformSettings.DeferredComponentLoaderViewEmbeddedResourceName != null
				    && Template.Key != platformSettings.DeferredComponentLoaderViewEmbeddedResourceName)
				{
					var textResource = EmbeddedResourceReader.ReadTextResource(
						platformSettings.DeferredComponentLoaderViewEmbeddedResourceName,
						GetType().Assembly);
					Template = new KeyValuePair<string, string>(
						platformSettings.DeferredComponentLoaderViewEmbeddedResourceName,
						textResource);
				}
			}
		}

		private static string GetCustomViewPath(string componentPath, string viewName, string extension = null)
		{
			var idx = componentPath.IndexOf(@"\flows\", StringComparison.InvariantCultureIgnoreCase);
			var pathWithFile = componentPath.Substring(idx);
			var path = string.Join('/', pathWithFile.Split('\\').SkipLast(1));
			var result = $"{path}/{viewName}";
			if (extension != null)
			{
				result = $"{result}.{extension}";
			}
			return result;
		}



		public async Task<IActionResult> InvokeDeferredAsync(string componentId, TComponentInput input, UiFlowScreenModel screenModel, Func<string, object, IActionResult> viewBuilder)
		{
			return await OnInvokeDeferredAsync(componentId, input, screenModel, viewBuilder);
		}

		protected virtual async Task<IActionResult> OnInvokeDeferredAsync(string componentId, TComponentInput input,
			UiFlowScreenModel screenModel, Func<string, object, IActionResult> viewBuilder)
		{
			if (!(PlatformSettings.IsDeferredComponentLoadEnabled && DeferComponentLoad))
			{
				return new BadRequestResult();
			}
			if (input == null)
			{
				input = new TComponentInput();
			}

			ComponentId = componentId;
			var data = await ResolveComponentDataAsync(input, screenModel);
			data.ScreenModel = screenModel;
			data.ComponentId = componentId;
			return viewBuilder(ViewName, data);
		}

		

		private async Task<IViewComponentResult> ResolveLoaderResultAsync(TComponentInput input,
			UiFlowScreenModel uiFlowScreenModel)
		{
			if (input == null) throw new ArgumentNullException(nameof(input));

			var loaderContent = Template.Value
				.Replace("[[componentId]]", ComponentId)
				.Replace("[[loadingPlaceholder]]", _loadingViewContent.Value)
				.Replace("[[urlPlaceholder]]", await ResolveUrl());

			return new HtmlContentViewComponentResult(new HtmlString(loaderContent));

			async Task<string> ResolveUrl()
			{


				var avoidBaseProps = typeof(TComponentModel).GetPropertiesFast(
						BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public |
						BindingFlags.GetProperty | BindingFlags.SetProperty, p => p.DeclaringType != typeof(TComponentModel),
						$"InheritedOf_{typeof(TComponentModel).FullName}")
					.Select(x => x.Name);
				var val = await HttpContext.EncryptedActionUrl(
					nameof(DeferredComponentPartialsController.Resolve)
					, typeof(DeferredComponentPartialsController).GetNameWithoutSuffix()
					, new
					{
						HandlerType = GetType().FullName,
						FlowHandler = uiFlowScreenModel.FlowHandler,
						ComponentId = ComponentId
					}
						.MergeObjects(input
							.ToExpandoObject(avoidBaseProps.ToArray()))
				);
				return val;
			}
		}

	}
}