﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Profiling;
using Ei.Rp.Web.DebugTools.Infrastructure.FlowDiagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using NLog;

namespace Ei.Rp.Web.DebugTools.Infrastructure.Middleware
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	public static class FlowStatesTracesMiddleware
	{
		public static IApplicationBuilder UseFlowStatesTracerPerRequest(this IApplicationBuilder builder,bool trackAnonymous=true)
		{
			return builder.UseMiddleware<Middleware>(trackAnonymous);
		}

		private class Middleware
		{
			private readonly IUserSessionProvider _userSessionProvider;
			private readonly RequestDelegate _next;
			private readonly IFlowChangesRecorder _flowChangesRecorder;
			private readonly bool _trackAnonymous;

			private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

			public Middleware(IUserSessionProvider userSessionProvider,RequestDelegate next, IFlowChangesRecorder flowChangesRecorder,bool trackAnonymous)
			{
				_userSessionProvider = userSessionProvider;
				_next = next;
				_flowChangesRecorder = flowChangesRecorder;
				_trackAnonymous = trackAnonymous;
			}

			public async Task Invoke(HttpContext context)
			{
				if ( 
					context.Request.Path.HasValue && context.Request.Path.Value.Contains("FlowDebugger", StringComparison.InvariantCultureIgnoreCase)
					|| !_trackAnonymous && _userSessionProvider.IsAnonymous())
				{
					await _next(context);
				}
				else
				{
					try
					{
						using (await _flowChangesRecorder.NewScopeAsync($"URL:{context.Request.GetDisplayUrl()}"))
						{
							await _next(context);
						}
					}
					catch (InvalidOperationException ex)
					{
						if (ex.Source == "Session")
						{
							await _next(context);
						}
						else throw;
					}
				}

			}
		}
	}
}