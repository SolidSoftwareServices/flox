using System.Diagnostics;
using System.Reflection;
using EI.RP.UiFlows.Mvc.Controllers;
using EI.RP.UiFlows.Mvc.ViewModels.Binders;
using Ei.Rp.Web.DebugTools;
using Ei.Rp.Web.DebugTools.Infrastructure.Middleware;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace EI.RP.UiFlows.Mvc.AppFeatures
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	public static class AppSetupExtensions
	{
		/// <summary>
		/// registers the ui flows to be used
		/// </summary>
		/// <typeparam name="TFlowTypesEnum"></typeparam>
		/// <param name="builder"></param>
		/// <param name="services"></param>
		/// <param name="enableFlowsDebugger">indicates if the flows debugger will be available at the relative url './flowdebugger/debugger' </param>
		/// <returns></returns>
		public static IMvcBuilder AddUiFlows<TFlowTypesEnum>(this IMvcBuilder builder, IServiceCollection services, bool enableFlowsDebugger)
		{
			return builder.AddUiFlows<TFlowTypesEnum, UiFlowController>(services,enableFlowsDebugger) ;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TFlowTypesEnum"></typeparam>
		/// <typeparam name="TFlowsController"></typeparam>
		/// <param name="builder"></param>
		/// <param name="services"></param>
		/// <param name="enableFlowsDebugger">indicates if the flows debugger will be available at the relative url './flowdebugger/debugger' </param>
		/// <returns></returns>
		public static IMvcBuilder AddUiFlows<TFlowTypesEnum,TFlowsController>(this IMvcBuilder builder, IServiceCollection services, bool enableFlowsDebugger) where TFlowsController : IUiFlowController
		{
			if (enableFlowsDebugger)
			{
				services.AddFlowDebugger();
			}
			services.Configure<RazorViewEngineOptions>(o =>
			{
				o.ViewLocationExpanders.Add(new UiFlowViewLocationExpander<TFlowTypesEnum>());
				o.ViewLocationExpanders.Add(new UiFlowDebugToolsViewLocationExpander());
				var fileProvider1 = new EmbeddedFileProvider(typeof(AppSetupExtensions).GetTypeInfo().Assembly);
				o.FileProviders.Add(fileProvider1);
			
				
			})
				;
			builder.AddMvcOptions(o =>
			{
				o.Conventions.Add(new UiFlowControllerRouteConvention<TFlowTypesEnum, TFlowsController>());
				o.ModelBinderProviders.Insert(0, new UiFlowStepDataModelBinderProvider());


			});

			return builder.ConfigureApplicationPartManager(m =>
				m.FeatureProviders.Add(new UiFlowControllerFeatureProvider()));
		}

	

	}
}
