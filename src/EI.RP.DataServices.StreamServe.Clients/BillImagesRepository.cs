using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Http.Clients;
using EI.RP.CoreServices.Profiling;
using EI.RP.CoreServices.Resiliency;
using NLog;

namespace EI.RP.DataServices.StreamServe.Clients
{
	class BillImagesRepository : IPdfOverlayImageRepository
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IPdfOverlayRepositorySettings _settings;
		private readonly IProfiler _profiler;
		private readonly IHttpClientBuilder _httpClientBuilder;

		public BillImagesRepository(IPdfOverlayRepositorySettings settings,IProfiler profiler,IHttpClientBuilder httpClientBuilder)
		{
			_settings = settings;
			_profiler = profiler;
			_httpClientBuilder = httpClientBuilder;
		}

		public async Task<StreamContent> GetImageStreamAsync(string fileName, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (fileName == null) throw new ArgumentNullException(nameof(fileName));
			using (var client = BuildClient(_settings.PdfOverlayImagesBaseUrl))
			{
				var response= await ResilientOperations.Default.RetryIfNeeded(async ()=>
				{
					var responseMessage = await client.GetAsync(fileName);
					responseMessage.EnsureSuccessStatusCode();
					return responseMessage;
				},cancellationToken,3);
				var output=new StreamContent(await response.Content.ReadAsStreamAsync());
				return output;
			}

			HttpClient BuildClient(string baseUrl)
			{
				return  _httpClientBuilder.Build(customizeHttpClientFunc: c =>
				{
					c.BaseAddress = new Uri(baseUrl);
					c.DefaultRequestHeaders.Accept.Clear();
				});
			}
		}
	}
}