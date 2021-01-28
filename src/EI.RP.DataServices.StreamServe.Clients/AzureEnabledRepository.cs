using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Authx;
using EI.RP.CoreServices.Http.Clients;
using EI.RP.CoreServices.Profiling;
using EI.RP.CoreServices.Resiliency;
using NLog;

namespace EI.RP.DataServices.StreamServe.Clients
{
	class AzureEnabledRepository: IStreamServeRepository
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IProfiler _profiler;
		private readonly IBearerTokenProvider _bearerTokenProvider;
		private readonly IHttpClientBuilder _httpClientBuilder;
		private readonly IStreamServeSettings _settings;

		public AzureEnabledRepository(IStreamServeSettings settings, IProfiler profiler,IBearerTokenProvider bearerTokenProvider,IHttpClientBuilder httpClientBuilder)
		{
			_settings = settings;
			_profiler = profiler;
			_bearerTokenProvider = bearerTokenProvider;
			_httpClientBuilder = httpClientBuilder;
		}

		public async Task<StreamContent> GetInvoiceFileStream(string invoiceDocNumber,CancellationToken cancellationToken = default(CancellationToken))
		{
			using (var client = await BuildClient(_settings.StreamServeUrl))
			{
				var response= await ResilientOperations.Default.RetryIfNeeded(async ()=>
				{
					var responseMessage = await client.GetAsync($"api/v1/InvoiceFiles/{invoiceDocNumber}", cancellationToken);
					responseMessage.EnsureSuccessStatusCode();
					return responseMessage;
				},cancellationToken);
				var output=new StreamContent(await response.Content.ReadAsStreamAsync());
				return output;
			}

		}

		public async Task<bool> IsHealthy(CancellationToken cancellationToken = default(CancellationToken))
		{
			using (var client = await BuildClient(_settings.StreamServeUrl))
			{
				try
				{
					var response = await client.GetAsync($"api/v1/InvoiceFiles/HealthStatus",cancellationToken);
					return response.StatusCode == HttpStatusCode.OK;
				}
				catch
				{
					return false;
				}
			}
		}

		

		private async Task< HttpClient> BuildClient(string baseUrl)
		{
			var getToken = _settings.StreamServeLiveBearerTokenProviderUrlAsync();
			return await _httpClientBuilder.BuildAsync(customizeHttpClientFunc: async c =>
			{
				c.BaseAddress = new Uri(baseUrl);
				c.DefaultRequestHeaders.Accept.Clear();

				await _bearerTokenProvider.AppendHeaders(c.DefaultRequestHeaders, await getToken);
			});
		}
	}
}
