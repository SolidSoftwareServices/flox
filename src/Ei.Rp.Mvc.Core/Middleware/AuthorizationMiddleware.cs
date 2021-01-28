using System;
#if !FrameworkDeveloper
using System.Diagnostics;
#endif
using System.Net;
using System.Threading.Tasks;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.Profiling;
using Ei.Rp.Mvc.Core.Controllers;
using Ei.Rp.Mvc.Core.System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NLog;

namespace Ei.Rp.Mvc.Core.Middleware
{
#if !FrameworkDeveloper
	
	[DebuggerStepThrough]
#endif
	public static class AuthorizationMiddleware
	{
		public static IApplicationBuilder UseAuthorizationMiddleware(this IApplicationBuilder builder, Type authorizationController, string authorizationAction, Type authenticationController, string authenticationAction)
		{
			return builder.UseMiddleware<Middleware>(authorizationController.GetNameWithoutSuffix(), authorizationAction,authenticationController.GetNameWithoutSuffix(),authenticationAction);
		}

		private class Middleware
		{
			private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
			private readonly IUserSessionProvider _userSessionProvider;

			private readonly RequestDelegate _next;
			private readonly string _authorizationController;
			private readonly string _authorizationAction;
			private readonly string _authenticationController;
			private readonly string _authenticationAction;
			private readonly IProfiler _profiler;
			 
			public Middleware(IUserSessionProvider userSessionProvider, RequestDelegate next, string authorizationController, string authorizationAction, string authenticationController, string authenticationAction,IProfiler profiler)
			{
				_userSessionProvider = userSessionProvider;
				_next = next;
				
				_authorizationController = authorizationController;
				_authorizationAction = authorizationAction;
				_authenticationController = authenticationController;
				_authenticationAction = authenticationAction;
				_profiler = profiler;
			}

			public async Task Invoke(HttpContext context)
			{
				async Task<bool> RedirectToLogin()
				{
					try
					{
						await _userSessionProvider.EndCurrentSession();
						var path = _userSessionProvider.IsAnonymous()
							? $"{_authenticationController}/{_authenticationAction}"
							: $"{_authorizationController}/{_authorizationAction}";
						var contextRequest = context.Request;
						var basePath = contextRequest.PathBase;
						context.Response.Redirect(
							$"{basePath}/{path}", false);
						return true;
					}
					catch (Exception ex)
					{
						Logger.Warn(()=>ex.ToString());
						return false;
					}
				}

				using (_profiler.RecordStep($"{nameof(AuthorizationMiddleware)}.{nameof(Invoke)}"))
				{
					try
					{
						await _next(context);
					}
					catch(Exception ex)
					{
						
						Logger.Error(()=>ex.ToString());
						if (_userSessionProvider.IsAnonymous())
						{
							if (!await RedirectToLogin())
							{
								throw;
							}
						}
						else
						{
							throw;
						}
					}

					var statusCode = context.Response.StatusCode;
					if (statusCode == (int) HttpStatusCode.Unauthorized
					    || (!statusCode.IsSuccessStatusCode() && !statusCode.IsRedirectionStatusCode() && _userSessionProvider.IsAnonymous()))
					{
						await RedirectToLogin();
					}
				}
			}
		}
	}
}