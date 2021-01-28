using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NLog;
using System.IO;
using System.Linq;
using S3.CoreServices.Profiling;
using Environment = System.Environment;

namespace S3.Mvc.Core.Middleware
{
#if !FrameworkDeveloper
	[DebuggerStepThrough]
#endif
	public static class RequestResponseLoggingMiddleware
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<Middleware>();
        }

        private class Middleware
        {
            private readonly RequestDelegate _next;
            private readonly IProfiler _profiler;
            private readonly IRequestPipelineCoreSettings _settings;
            private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

            public Middleware(RequestDelegate next, IProfiler profiler, IRequestPipelineCoreSettings settings)
            {
	            _next = next;
	            _profiler = profiler;
	            _settings = settings;
            }

            public async Task Invoke(HttpContext context)
            {
				using (_profiler.RecordStep(context.Request.Path))
				{
					context.Items["RequestId"] = Guid.NewGuid().ToString();
					Logger.Debug(() => BuildRequestLogMessage(context));
					try
					{
						if (_settings.IsRequestVerboseLoggingEnabled)
						{
							await InvokeNextAndLogResponse(context);
						}
						else
						{
							await _next(context);
							Logger.Debug(() => BuildResponseLogMessage(context));
						}

					}
					catch (Exception ex)
					{
						Logger.Error(() => BuildRequestLogMessage(context, ex));
						throw;
					}
				}
            }

            private string BuildResponseLogMessage(HttpContext context)
			{
				return string.Join(Environment.NewLine,
					   $"Response:",
					   $"RequestId: {context.Items["RequestId"]}",
					   $"Request: {context.Request.Path}{context.Request.QueryString}",
					   $"ContentType: {context.Response.ContentType}",
					   $"StatusCode: {context.Response.StatusCode}");
			}

			private async Task InvokeNextAndLogResponse(HttpContext context)
			{
				var originalStream = context.Response.Body;
				var bodyAsText = string.Empty;
				try
				{
					using (var buffer = new MemoryStream())
					{
						context.Response.Body = buffer;

						await _next.Invoke(context);

						buffer.Seek(0, SeekOrigin.Begin);
						using (var bufferReader = new StreamReader(buffer))
						{
							bodyAsText = await bufferReader.ReadToEndAsync();
							Logger.Debug(() => string.Join(Environment.NewLine,
								Environment.NewLine,
								$"Response:",
								$"RequestId: {context.Items["RequestId"]}",
								$"Request: {context.Request.Path}{context.Request.QueryString}",
								$"ContentType: {context.Response.ContentType}",
								$"Headers: {string.Join(Environment.NewLine, context.Response.Headers.Select(x => $"{x.Key}={x.Value}"))}",
								$"Body: {bodyAsText}"));

							buffer.Seek(0, SeekOrigin.Begin);

							await buffer.CopyToAsync(originalStream);
							context.Response.Body = originalStream;
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Error(ex);
					throw;
				}
			}

			private string BuildRequestLogMessage(HttpContext context, Exception ex = null)
            {
	            var bodyAsText = string.Empty;
				var formStr = string.Empty;

				if ((_settings.IsRequestVerboseLoggingEnabled || ex != null) && context.Request.Method != "GET")
				{
					try
					{
						formStr = string.Join(Environment.NewLine,
							context.Request.Form?.Select(x => $"{x.Key}={x.Value}") ?? new string[0]);

						context.Request.EnableBuffering();

						using (var bodyReader = new StreamReader(context.Request.Body))
						{
							bodyAsText = bodyReader.ReadToEnd();
							context.Request.Body.Position = 0;
						}
					}
					catch (Exception e)
					{
						Logger.Error(() => e.ToString());
					}
				}

				return string.Join(Environment.NewLine,
					$"Request:",
					$"RequestId: {context.Items["RequestId"]}",
					$"Request: {context.Request.Path}{context.Request.QueryString}",
					$"ContentType: {context.Request.ContentType}",
					!string.IsNullOrEmpty(bodyAsText)? $"Body: {bodyAsText}" : string.Empty,
					!string.IsNullOrEmpty(formStr) ? $"Form: {formStr}" : string.Empty,
					$"{ex}");
			}
		}
    }
}