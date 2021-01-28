using System;
using S3.Web.DebugTools.Areas.FlowDebugger.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace S3.Web.DebugTools.Infrastructure.Middleware
{

    public static class StartupExtensions
    {        
        public static IServiceCollection AddFlowDebugger(this IServiceCollection services)
        {
			
			services.ConfigureOptions(typeof(ViewConfigureOptions));
			services.ConfigureOptions(typeof(ContentConfigureOptions));
			return services;
        }

	    public static IApplicationBuilder UseMvcWithUiFlowsDebugger(this IApplicationBuilder builder, Action<IRouteBuilder> configureRoutes)
	    {

		    builder
				.UseFlowStatesTracerPerRequest(trackAnonymous:false)
				.UseMvc(RoutesWithDebugger);
			    
			return builder;

		    void RoutesWithDebugger(IRouteBuilder routes)
		    {
			   
				configureRoutes(routes);
				routes.MapRoute(
					name: "flowDebuggerRoute",
					template: $"{{area:exists}}/{{controller={nameof(DebuggerController).Replace("Controller",string.Empty)}}}/{{action=Index}}");
			}
	    }
	}
}
