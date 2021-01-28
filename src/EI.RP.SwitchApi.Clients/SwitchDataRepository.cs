using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Authx;
using EI.RP.CoreServices.Http.Clients;
using EI.RP.CoreServices.Profiling;
using EI.RP.CoreServices.Serialization;
using EI.RP.DataModels.Switch;
using EI.RP.DataServices;
using NLog;

namespace EI.RP.SwitchApi.Clients
{
	internal class SwitchDataRepository : ISwitchDataRepository
	{
		private readonly ISwitchApiSettings _settings;
		private readonly IProfiler _profiler;
		private readonly IBearerTokenProvider _bearerTokenProvider;
		private readonly IHttpClientBuilder _httpClientBuilder;

		public SwitchDataRepository(ISwitchApiSettings settings, IProfiler profiler,IBearerTokenProvider bearerTokenProvider,IHttpClientBuilder httpClientBuilder)
		{
			_settings = settings;
			_profiler = profiler;
			_bearerTokenProvider = bearerTokenProvider;
			_httpClientBuilder = httpClientBuilder;
		}

		public async Task<IEnumerable<DiscountDto>> GetDiscountsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			return (await _GetAsync<IEnumerable<DiscountDto>>("0.1/en/existingcustomerpriceplan/getallexistingcustomerpriceplans",OnError, cancellationToken))
				.ToArray();

			string OnError(HttpResponseMessage response)
			{
				return $"{nameof(SwitchDataRepository)}.{nameof(GetDiscountsAsync)}." +
				       $"returned a status code: {response.StatusCode} - {response.ReasonPhrase}";
			}

		
		}
	
		public async Task<AddressDetailDto> GetAddressDetailFromMprn(string mprn, CancellationToken cancellationToken = default(CancellationToken))
		{

			return await _GetAsync<AddressDetailDto>($"0.1/en/mprn/getaddressdetailsfrommprn?mprn={mprn}", OnError, cancellationToken);

			string OnError(HttpResponseMessage response)
			{
				return $"{nameof(SwitchDataRepository)}.{nameof(GetAddressDetailFromMprn)} mprn: {mprn}." +
				       $"returned a status code: {response.StatusCode} - {response.ReasonPhrase}";
			}
		}

		public async Task<IEnumerable<RegisterDto>> GetRegisterDetailFromMprn(string mprn, CancellationToken cancellationToken = default(CancellationToken))
		{
			return await _GetAsync<IEnumerable<RegisterDto>>($"0.1/en/residentialmeterregister/{mprn}", OnError, cancellationToken);

			string OnError(HttpResponseMessage response)
			{
				return $"{nameof(SwitchDataRepository)}.{nameof(GetRegisterDetailFromMprn)} mprn: {mprn}." +
				       $"returned a status code: {response.StatusCode} - {response.ReasonPhrase}";
			}
		}
		private async Task<TResult> _GetAsync<TResult>(string url, Func<HttpResponseMessage, string> onError,
			CancellationToken cancellationToken)
		{
			using (var client = await BuildClient(cancellationToken))
			{
				using (var response = await client.GetAsync(url,cancellationToken))
				{
					if (response.IsSuccessStatusCode)
					{
						var jsonResult = await response.Content.ReadAsStringAsync();
						return jsonResult.JsonToObject<TResult>();
					}

					throw new HttpRequestException(onError(response));
				}
			}
		}
	
		private async Task<HttpClient> BuildClient(CancellationToken cancellationToken)
		{
			var getToken = _settings.SwitchApiBearerTokenProviderUrlAsync();

			return await _httpClientBuilder.BuildAsync(customizeHttpClientFunc: async c =>
			{
				c.BaseAddress = new Uri(_settings.SwitchApiEndPoint);
				c.DefaultRequestHeaders.Accept.Clear();
				c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				await _bearerTokenProvider.AppendHeaders(c.DefaultRequestHeaders,
					await getToken, cancellationToken);
			});

		}
	}
}