using System;
using System.Threading.Tasks;
using Ei.Rp.Mvc.Core.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NLog;
#if !FrameworkDeveloper
using System.Diagnostics;
#endif
namespace Ei.Rp.Mvc.Core.Middleware
{
#if !FrameworkDeveloper

	[DebuggerStepThrough]
#endif
	public static class ErrorHandlingMiddleware
	{
		public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder,
			string errorHandlingPath)
		{
			if(!errorHandlingPath.StartsWith('/')) throw new ArgumentException("Invalid error path. Must start with '/'");
			return builder.UseMiddleware<Middleware>(errorHandlingPath);
		}

		private class Middleware
		{
			private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
			private RequestDelegate next;
			private readonly string _errorHandlingPath;

			public Middleware(RequestDelegate next, string errorHandlingPath)
			{
				this.next = next;
				_errorHandlingPath = errorHandlingPath;
			}

			public async Task Invoke(HttpContext context)
			{
				try
				{
					await next(context);
					if (context.Response.StatusCode >= 400)
					{
						await HandleExceptionAsync(context);
					}
				}
				catch (Exception ex)
				{
					Logger.Error(() => ex.ToString());
					await HandleExceptionAsync(context, true);
				}
			}

			private async Task HandleExceptionAsync(HttpContext context, bool isUnhandled = false)
			{
				if (isUnhandled && !context.Response.HasStarted)
				{
					var path =
						$"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}{_errorHandlingPath}";
					context.Response.Redirect(path, false);
				}
			}
		}
	}
}