using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Profiling;
using EI.RP.CoreServices.Serialization;
using NLog;

namespace EI.RP.CoreServices.Http.Clients
{
	public static class HttpClientHandlerExtensions
	{
		public static HttpMessageHandler AddLoggingToPipeline(this HttpMessageHandler handler, IProfiler profiler)
		{
			return new LoggingHandler(profiler)
			{
				InnerHandler = handler
			};
		}

		private class LoggingHandler : DelegatingHandler
		{
			private static readonly ILogger Logger = LogManager.GetLogger("HttpRequestsToDependencies");

			private readonly IProfiler _profiler;

			public LoggingHandler(IProfiler profiler)
			{
				_profiler = profiler;
			}

			protected override async Task<HttpResponseMessage> SendAsync(
				HttpRequestMessage request,
				CancellationToken cancellationToken)
			{
				if (request.Method == HttpMethod.Delete && request.RequestUri.PathAndQuery.Contains("/$links/"))
				{
					//not supported by sap
					return new HttpResponseMessage(HttpStatusCode.Accepted);
				}

				var logPrefix = $" - OutgoingRequestId:{Guid.NewGuid().ToString()} {Environment.NewLine}";
				using (_profiler.Profile(request.Method.Method, request.RequestUri.ToString()))
				{
					Logger.Trace(() => $"{logPrefix}Request:{LogRequest(request).GetAwaiter().GetResult()}");
					try
					{
						var response = await base.SendAsync(request, cancellationToken);
						await FixApiM(response);
						Logger.Trace(() =>
							$"{logPrefix}{LogResponse(response).GetAwaiter().GetResult()}{Environment.NewLine}");
						return response;
					}
					catch (Exception ex)
					{
						Logger.Error(() =>
							$"{logPrefix}Failed to get response: {ex}{Environment.NewLine}Original Request:{Environment.NewLine}{LogRequest(request).GetAwaiter().GetResult()}");
						throw;
					}
				}
			}

			private async Task FixApiM(HttpResponseMessage response)
			{
				if (response.RequestMessage.Method == HttpMethod.Post &&
				    response.RequestMessage.RequestUri.PathAndQuery.ToLower().Contains("/$batch"))
				{
					var contentType = response.Content.Headers.ContentType.ToString()?.Split('.').LastOrDefault();
					if (contentType != null)
					{
						var headerParts = contentType.Split(';');
						if (headerParts.Length >= 2)
						{
							var parts = headerParts[1].Split('=');
							if (parts.FirstOrDefault()?.Trim() == "boundary")
							{
								var token = parts.Last();
								string body;
								using(var streamReader = new StreamReader(await response.Content.ReadAsStreamAsync()))
								{
									body = await streamReader.ReadToEndAsync();
								}
								parts = body.Split(new[] {$"--{token}--", $"--{token}"},
									StringSplitOptions.RemoveEmptyEntries).Where(x=>x!=Environment.NewLine).ToArray();
								var resultParts = new List<string>();
								foreach (var part in parts)
								{
									var lines = part.Split(new[] {Environment.NewLine}, StringSplitOptions.None)
										.ToList();
									//removes first and last line
									lines = lines.GetRange(1, lines.Count - 2);
									var firstSeparatorIndex = lines.IndexOf(string.Empty);
									var secondSection = lines.GetRange(firstSeparatorIndex+1,
										lines.Count - firstSeparatorIndex -1);

									var firstContentLengthIndex = lines.FindIndex(x => x?.StartsWith("Content-Length") ?? false);
									var secondContentLengthIndex = lines.FindLastIndex(x => x?.StartsWith("Content-Length") ?? false);
									var newlineLength = Environment.NewLine.Length;
									lines[firstContentLengthIndex] = $"Content-Length: {secondSection.Sum(x => x.Length+newlineLength)-newlineLength}";

									var secondSeparatorIndex = secondSection.IndexOf(string.Empty);
									var bodySection = secondSection.GetRange(secondSeparatorIndex+1,
										secondSection.Count - secondSeparatorIndex - 1);
									if (secondContentLengthIndex != firstContentLengthIndex)
									{
										lines[secondContentLengthIndex] = $"Content-Length: {bodySection.Sum(x => x.Length)}";
									}
									resultParts.Add(string.Join(Environment.NewLine, lines));
								}
								
								var sb = new StringBuilder(string.Join($"--{token}{Environment.NewLine}", resultParts));
								sb.AppendLine($"{Environment.NewLine}--{token}--");
								var content = $"--{token}{Environment.NewLine}{sb}";
								response.Content = new StringContent(content);
								response.Content.Headers.AddOrUpdateExisting("Content-Type",contentType);
							}
						}

					}
				}
			}

			private static readonly MediaTypeHeaderValue XmlContentType = MediaTypeHeaderValue.Parse("application/xml");
			private static readonly MediaTypeHeaderValue JsonContentType = MediaTypeHeaderValue.Parse("application/json");
			private static readonly MediaTypeHeaderValue PdfContentType = MediaTypeHeaderValue.Parse("application/pdf");
			private static readonly MediaTypeHeaderValue JpegContentType = MediaTypeHeaderValue.Parse("image/jpeg");
			private async Task<string> LogResponse(HttpResponseMessage response)
			{
				var sb = new StringBuilder(Environment.NewLine);
				sb.AppendLine("*************************************************************************");
				sb.AppendLine($"Response of: {response.RequestMessage.RequestUri}");
				sb.AppendLine(response.ToString());
				sb.AppendLine(".........................................................................");
				sb.AppendLine("Content:");
				if (response.Content != null)
				{

					sb.AppendLine(response.Content.Headers.ToString());

					var contentType = response.Content.Headers.ContentType;
					if (new[]{XmlContentType,JsonContentType}.Contains( contentType))
					{
						string content;
						using (var streamReader = new StreamReader(await response.Content.ReadAsStreamAsync()))
						{
							content = await streamReader.ReadToEndAsync();
						}

						response.Content = new StringContent(content);
						response.Content.Headers.ContentType = contentType;

						if (response.Content.Headers.ContentType == XmlContentType)
						{
							content = content.XmlPrettyPrint();
						}
						else if (response.Content.Headers.ContentType == JsonContentType)
						{
							content = content.JsonPrettyPrint();
						}

						sb.AppendLine(content);
					}
				}
				else
				{
					sb.AppendLine("(empty)");
				}

				sb.AppendLine("==================================================================================");
				sb.AppendLine();
				return sb.ToString();
			}

			private async Task<string> LogRequest(HttpRequestMessage request)
			{
				var sb = new StringBuilder(Environment.NewLine);
				sb.AppendLine("*************************************************************************");
				sb.AppendLine(request.ToString());
				sb.AppendLine(".........................................................................");
				sb.AppendLine("Content:");
				if (request.Content != null)
				{

					sb.AppendLine(request.Content.Headers.ToString());
					sb.AppendLine(await request.Content.ReadAsStringAsync());
				}
				else
				{
					sb.AppendLine("(empty)");
				}

				sb.AppendLine("==================================================================================");
				sb.AppendLine();
				return sb.ToString();
			}
		}
	}
}