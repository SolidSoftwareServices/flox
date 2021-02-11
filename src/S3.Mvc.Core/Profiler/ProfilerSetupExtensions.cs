using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace S3.Mvc.Core.Profiler
{

	public static class ProfilerSetupExtensions
	{
		
		public static void AddProfiler(this IServiceCollection services, Func<bool> isProfilerEnabled)
		{
			

			if (isProfilerEnabled())
			{
				services.AddMiniProfiler(options =>
				{
					options.RouteBasePath = "/profiler";
					options.ShouldProfile = r => true;
					options.PopupMaxTracesToShow = 1024;
					options.PopupShowTimeWithChildren = true;
					options.PopupShowTrivial = true;
					options.ShowControls = true;
					
				});
			}
		}

		public static IApplicationBuilder UseProfiler(this IApplicationBuilder app, Func<bool> isProfilerEnabled)
		{
			if (!isProfilerEnabled()) return app;
			return app.UseMiniProfiler();
		}

	}
}
