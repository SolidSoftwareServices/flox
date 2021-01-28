using System.Diagnostics;
using System.Reflection;
using S3.UiFlows.Mvc.Controllers;
using S3.UiFlows.Mvc.ViewModels.Binders;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace S3.UiFlows.Mvc.AppFeatures
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
		public static IMvcBuilder AddUiFlows<TFlowTypesEnum>(this IMvcBuilder builder, IServiceCollection services)
		{
			return builder.AddUiFlows<TFlowTypesEnum, UiFlowController>(services) ;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TFlowTypesEnum"></typeparam>
		/// <typeparam name="TFlowsController"></typeparam>
		/// <param name="builder"></param>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IMvcBuilder AddUiFlows<TFlowTypesEnum,TFlowsController>(this IMvcBuilder builder, IServiceCollection services) where TFlowsController : IUiFlowController
		{
			

			services.Configure<RazorViewEngineOptions>(o =>
			{
				o.ViewLocationExpanders.Add(new UiFlowViewLocationExpander<TFlowTypesEnum>());
			});
			services.Configure<MvcRazorRuntimeCompilationOptions>(opts =>
			{
				var fileProvider = new EmbeddedFileProvider(typeof(AppSetupExtensions).GetTypeInfo().Assembly);
				opts.FileProviders.Add(fileProvider);
			});
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
