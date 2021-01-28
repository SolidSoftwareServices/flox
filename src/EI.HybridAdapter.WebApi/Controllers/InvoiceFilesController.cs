using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EI.HybridAdapter.WebApi.Infrastructure.Settings;
using EI.RP.CoreServices.Http.Clients;
using EI.RP.CoreServices.Profiling;
using EI.RP.CoreServices.Resiliency;
using Ei.Rp.Mvc.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace EI.HybridAdapter.WebApi.Controllers
{
	/// <summary>
	/// Serves invoice files adapting stream serve on-prem to cloud
	/// </summary>
	[Route("api/v{version:apiVersion}/[controller]")]
	[ApiController]
	[ApiVersion("1.0")]
	[Authorize]
	[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
	public class InvoiceFilesController : ControllerBase
	{
		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

		private readonly IStreamServeSettings _settings;
		private readonly IProfiler _profiler;

		public InvoiceFilesController(IStreamServeSettings settings, IProfiler profiler)
		{
			_settings = settings;
			_profiler = profiler;
		}

		/// <summary>
		/// Retrieves a result indicating whether the service is available or down
		/// </summary>
		/// <returns></returns>
		[HttpGet(nameof(HealthStatus))]
		[ProducesResponseType((int)HttpStatusCode.OK)]
		[ProducesResponseType((int)HttpStatusCode.ServiceUnavailable)]
		public async Task<IActionResult> HealthStatus()
		{
			
			try
			{
				await ResilientOperations.Default.RetryIfNeeded(async () =>
				{
					await Get("health_check_test");
				
				}, maxAttempts: 2);
			}
			catch
			{
				return new HttpResponseMessageResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
			}

			return Ok();
		}

		/// <summary>
		/// Retrieves an invoice file 
		/// </summary>
		/// <param name="invoiceDocNumber">The invoice number to be retrieved</param>
		/// <returns></returns>
		[HttpGet("{invoiceDocNumber}")]
		[ProducesResponseType((int) HttpStatusCode.OK)]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Get([Required]string invoiceDocNumber)
		{
			if (string.IsNullOrWhiteSpace(invoiceDocNumber)) return BadRequest();

			var streamLiveTask = Get(invoiceDocNumber, _settings.StreamServeLive);
			var streamArchiveTask = Get(invoiceDocNumber, _settings.StreamServeArchive);
			var stream = await streamLiveTask;
			if (stream?.Headers.ContentLength == null || stream.Headers.ContentLength.Value < 256)
			{
				stream = await streamArchiveTask;
			}
			if (stream?.Headers.ContentLength == null || stream.Headers.ContentLength.Value < 256)
			{
				return NotFound();
			}
			 
			var response = new HttpResponseMessage(HttpStatusCode.OK);
			response.Content = stream;
			return new HttpResponseMessageResult(response);
		}

		private async Task<StreamContent> Get(string invoiceDocNumber, string url)
		{
			using (var client = BuildClient(url))
			{
				var response = await client.GetAsync(
					$"urlsearch?i1=Invoice_Number&s1=21&type=STANDARD_INVOICE&v1={invoiceDocNumber}");
				response.EnsureSuccessStatusCode();
				var result = new StreamContent(await response.Content.ReadAsStreamAsync());
				return result;
			}
		}


		private HttpClient BuildClient(string baseUrl)
		{
			var handler = new HttpClientHandler
			{
				ServerCertificateCustomValidationCallback = (message, certificate2, arg3, arg4) => true,
				DefaultProxyCredentials = CredentialCache.DefaultCredentials,
				Credentials = new NetworkCredential(_settings.StreamServeUserName, _settings.StreamServePassword)
			}.AddLoggingToPipeline(_profiler);
			var client = new HttpClient(handler);
			client.BaseAddress = new Uri(baseUrl);
			client.DefaultRequestHeaders.Accept.Clear();
			return client;
		}
	}
}
