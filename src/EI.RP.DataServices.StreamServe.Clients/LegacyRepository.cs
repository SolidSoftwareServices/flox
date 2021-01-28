using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Http.Clients;
using EI.RP.CoreServices.Profiling;
using NLog;

namespace EI.RP.DataServices.StreamServe.Clients
{
	class LegacyRepository: IStreamServeRepository
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IProfiler _profiler;
		private readonly IHttpClientBuilder _httpClientBuilder;
		private readonly ILegacyStreamServeSettings _settings;

		public LegacyRepository(ILegacyStreamServeSettings settings, IProfiler profiler,IHttpClientBuilder httpClientBuilder)
		{
			_settings = settings;
			_profiler = profiler;
			_httpClientBuilder = httpClientBuilder;
		}

		public async Task<StreamContent> GetInvoiceFileStream(string invoiceDocNumber, CancellationToken cancellationToken = default(CancellationToken))
		{

			var  streamLiveTask = Get(invoiceDocNumber, _settings.StreamServeLive, cancellationToken);
			var streamArchiveTask = Get(invoiceDocNumber, _settings.StreamServeArchive, cancellationToken);
			var stream = await streamLiveTask;
			if (stream == null || stream.Length < 256)
			{
				stream = await streamArchiveTask;
			}

			return new StreamContent(stream);

		}

		public async Task<bool> IsHealthy(CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				using (var stream = await GetInvoiceFileStream("LegacyIsHealthy", cancellationToken)) ;
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}

			

		}

		private async Task<Stream> Get(string invoiceDocNumber, string url, CancellationToken cancellationToken = default(CancellationToken))
		{
			Stream output=new MemoryStream();
			using (var client = await BuildClient(url,cancellationToken))
			{
				var response= await client.GetAsync(
					$"urlsearch?i1=Invoice_Number&s1=21&type=STANDARD_INVOICE&v1={invoiceDocNumber}",cancellationToken);
				response.EnsureSuccessStatusCode();
				var result= await response.Content.ReadAsStreamAsync();
				await result.CopyToAsync(output);
			}

			output.Seek(0L, SeekOrigin.Begin);
			return output;
		}

		private async Task<HttpClient> BuildClient(string baseUrl,
			CancellationToken cancellationToken = default(CancellationToken))
		{

			return await _httpClientBuilder.BuildAsync(
				async c => c.Credentials =
					new NetworkCredential(_settings.StreamServeUserName, _settings.StreamServePassword), async c =>
				{
					c.BaseAddress = new Uri(baseUrl);
					c.DefaultRequestHeaders.Accept.Clear();

				});
		}
	}
}
