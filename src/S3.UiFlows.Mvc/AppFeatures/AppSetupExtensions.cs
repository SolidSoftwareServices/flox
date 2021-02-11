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
	public static class AppSetupExtensions
	{
		/// <summary>
		/// registers the ui flows to be used
		/// </summary>
		/// <typeparam name="TFlowTypesEnum"></typeparam>
		/// <param name="builder"></param>
		/// <param name="services"></param>
		/// <param name="flowsAssembly"></param>
		/// <param name="flowsRootNamespace"></param>
		/// <param name="flowsRootPath"></param>
		/// <returns></returns>
		public static IMvcBuilder AddUiFlows(this IMvcBuilder builder, IServiceCollection services,
			Assembly flowsAssembly, string flowsRootNamespace, string flowsRootPath)
		{
			return builder.AddUiFlows< UiFlowController>(services,flowsAssembly,flowsRootNamespace, flowsRootPath) ;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="TFlowTypesEnum"></typeparam>
		/// <typeparam name="TFlowsController"></typeparam>
		/// <param name="builder"></param>
		/// <param name="services"></param>
		/// <param name="flowsAssembly"></param>
		/// <param name="flowsRootNamespace"></param>
		/// <param name="flowsRootPath"></param>
		/// <returns></returns>
		public static IMvcBuilder AddUiFlows<TFlowsController>(this IMvcBuilder builder, IServiceCollection services,
			Assembly flowsAssembly, string flowsRootNamespace, string flowsRootPath) where TFlowsController : IUiFlowController
		{
			FlowsRegistry.Instance.Load(flowsAssembly, flowsRootNamespace,flowsRootPath);
			services.Configure<RazorViewEngineOptions>(o =>
			{
				o.ViewLocationExpanders.Add(new UiFlowViewLocationExpander(FlowsRegistry.Instance));
			});
			services.Configure<MvcRazorRuntimeCompilationOptions>(opts =>
			{
				var fileProvider = new EmbeddedFileProvider(typeof(AppSetupExtensions).GetTypeInfo().Assembly);
				opts.FileProviders.Add(fileProvider);
			});
			;
			builder.AddMvcOptions(o =>
			{
				//HERE
				o.Conventions.Add(new UiFlowControllerRouteConvention< TFlowsController>(FlowsRegistry.Instance));
				o.ModelBinderProviders.Insert(0, new UiFlowStepDataModelBinderProvider());


			});

			return builder.ConfigureApplicationPartManager(m =>
				m.FeatureProviders.Add(new UiFlowControllerFeatureProvider()));
		}

	

	}
}
