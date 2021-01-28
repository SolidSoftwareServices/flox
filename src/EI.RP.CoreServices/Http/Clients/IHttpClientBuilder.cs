using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EI.RP.CoreServices.Profiling;

namespace EI.RP.CoreServices.Http.Clients
{
	public interface IHttpClientBuilder
	{
		Task<HttpClient> BuildAsync(Func<HttpClientHandler, Task> customizeHandlerAsync=null,Func<HttpClient,Task> customizeHttpClientFunc=null);
		HttpClient Build(Action<HttpClientHandler> customizeHandler=null,Action<HttpClient> customizeHttpClientFunc=null);
	}

	class DefaultHttpClientBuilder : IHttpClientBuilder
	{
		private readonly IProfiler _profiler;

		public DefaultHttpClientBuilder(IProfiler profiler)
		{
			_profiler = profiler;
		}
		public HttpClient Build(Action<HttpClientHandler> customizeHandler = null, Action<HttpClient> customizeHttpClientFunc = null)
		{
			return BuildAsync(customizeHandler == null ? (Func<HttpClientHandler, Task>)null : (HttpClientHandler c) =>
				{
					customizeHandler(c);
					return Task.CompletedTask;
				}
				,customizeHttpClientFunc == null ? (Func<HttpClient, Task>)null : (HttpClient c) =>
				{
					customizeHttpClientFunc(c);
					return Task.CompletedTask;
				}
			).GetAwaiter().GetResult();
			
		}

		public async Task<HttpClient> BuildAsync(Func<HttpClientHandler, Task> customizeHandlerAsync = null,
			Func<HttpClient, Task> customizeHttpClientFunc = null)
		{
			var httpClientHandler = new HttpClientHandler
			{
				ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true,
				DefaultProxyCredentials = CredentialCache.DefaultCredentials,
				AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
				CookieContainer = new CookieContainer()
			};
			await (customizeHandlerAsync?.Invoke(httpClientHandler) ?? Task.CompletedTask);
			var handler = httpClientHandler.AddLoggingToPipeline(_profiler);

			var client = new HttpClient(handler);
			await (customizeHttpClientFunc?.Invoke(client) ?? Task.CompletedTask);

			return client;
		}


	}
}