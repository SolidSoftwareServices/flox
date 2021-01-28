using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.DeliveryPipeline.ManualTesting;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Profiling;
using Ei.Rp.Mvc.Core.Middleware;
using Ei.Rp.Mvc.Core.System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NLog;

namespace EI.RP.WebApp.Infrastructure.Middleware
{
	public static class ContinuousDeliveryMiddleware
	{
		public static IApplicationBuilder UseContinuousDeliveryMiddleware(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<Middleware>();
		}

		private class Middleware
		{
			private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

			private readonly RequestDelegate _next;
			
			public Middleware(RequestDelegate next)
			{
			
				_next = next;
			
			}

			public async Task Invoke(HttpContext context)
			{
				var sw = new Stopwatch();
				sw.Start();
				try
				{
					await _next(context);
				}
				finally
				{
					var simulateAppWorkloadDelaySeconds = context.Resolve<IConfigurableTestingItems>().SimulateAppWorkloadDelaySeconds;
					if (simulateAppWorkloadDelaySeconds > 0)
					{
						Logger.Warn(() => $"Test settings configured the app to lag {simulateAppWorkloadDelaySeconds}");
						var lag = TimeSpan.FromSeconds(simulateAppWorkloadDelaySeconds);
						var toWait = lag.Subtract(sw.Elapsed);
					
						Thread.Sleep(toWait);
					}
					sw.Stop();
				}
			}
		}
	}
}