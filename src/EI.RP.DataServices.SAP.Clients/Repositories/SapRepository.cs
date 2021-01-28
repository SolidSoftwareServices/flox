using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using EI.RP.CoreServices.Http.Clients;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.OData.Client;
using EI.RP.CoreServices.Ports.OData;
using EI.RP.CoreServices.Serialization;
using EI.RP.CoreServices.System;
using EI.RP.DataServices.SAP.Clients.ErrorHandling;
using EI.RP.DataServices.SAP.Clients.Infrastructure;
using EI.RP.DataServices.SAP.Clients.Infrastructure.Session;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Simple.OData.Client;

namespace EI.RP.DataServices.SAP.Clients.Repositories
{
	internal abstract class SapRepository: ODataRepository
	{
		private readonly TimeSpan _requestTimeout;
		private readonly SapRepositoryOptions _options;
		private readonly IHttpClientBuilder _httpClientBuilder;

		protected SapRepository(SapRepositoryOptions options,IHttpClientBuilder httpClientBuilder)
			: base(options)
		{
			_requestTimeout = options.RequestTimeout;
			_options = options;
			_httpClientBuilder = httpClientBuilder;
		}

		protected ISapSessionDataRepository SapSessionData => _options.SapSessionData;
		protected ISapResultStatusHandler ApiResultHandler => _options.ApiResultHandler;
		protected IUserSessionProvider UserSessionProvider => _options.UserSessionProvider;

		public override async Task<string> GetName()
		{
			try
			{
				return (await base.GetEntityContainerName()).Replace("UTILITIES", string.Empty).ToPascalCase();
			}
			catch (HttpRequestException)
			{
				return GetTemporalName();
			}
		}

		protected virtual string GetTemporalName()
		{
			return GetType().Name;
		}

		protected override ODataClientSettings BuildClientSettings(string endPointUrl)
		{
			var oDataClientSettings = new ODataClientSettings(BuildHttpClient())
			{
				BeforeRequestAsync = PrepareRequest,
				AfterResponseAsync = response => ApiResultHandler.EnsureSuccessfulResponse(response),
				IgnoreUnmappedProperties = true,
				PreferredUpdateMethod = ODataUpdateMethod.Put,
			};
			
			return oDataClientSettings;
			HttpClient BuildHttpClient()
			{
				return _httpClientBuilder.Build(customizeHttpClientFunc: c =>
				{
					c.Timeout = _requestTimeout;
					c.BaseAddress = new Uri(endPointUrl, UriKind.Absolute);
				});
			}
		}

		protected async Task PrepareRequest(HttpRequestMessage httpRequest)
		{
			var csrf = default(string);
			var cookieJson = default(string);

			csrf = SapSessionData.SapCsrf;
			cookieJson = SapSessionData.SapJsonCookie;
			var cookieCollection = cookieJson?.JsonToObject<List<Cookie>>();

			var cookieString = new StringBuilder();
			if (cookieCollection != null)
			{
				foreach (var item in cookieCollection)
				{
					var name = item.Name;
					var val = item.Value;
					cookieString.Append($"{name}={val};");
				}

				httpRequest.Headers.Add("Cookie", cookieString.ToString());
			}

			if (httpRequest.Method != HttpMethod.Get)
			{
				ResolveCsrfHeaders(httpRequest.Headers, csrf);
			}

			httpRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
			await Options.AppendHeaders(httpRequest.Headers);
			
		}

		protected virtual void ResolveCsrfHeaders(HttpRequestHeaders headers, string csrf)
		{
			const string xRequestedWith = "X-Requested-With";
			if (string.IsNullOrWhiteSpace(csrf) && !headers.Contains(xRequestedWith))
				headers.Add(xRequestedWith, "XMLHttpRequest");
			headers.AddOrUpdateExisting("x-csrf-token", csrf);
		}

		protected async Task<TResult> SapExecute<TResult>(Func<HttpClient, HttpClientHandler, Task<TResult>> payload,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			HttpClientHandler handler=null;
			using (var client = _httpClientBuilder.Build(h => handler=h,  c =>
			{
				c.DefaultRequestHeaders.Clear();
				c.DefaultRequestHeaders.AddIfNotExistsSeparatedByComma("x-csrf-token", "fetch");
			}))
			{
				await Options.AppendHeaders(client.DefaultRequestHeaders, cancellationToken);
				return await payload(client, handler);
			}
		}

		[Obsolete("Deep updates are supported from odata v4, but that is what the SAP team has implemented only.")]
		private async Task<TDto> CustomAddAsJson<TDto>(TDto newEntity,bool withResult=true) where TDto : ODataDtoModel
		{
			using (Profiler.Profile(ProfileCategoryId, $"{typeof(TDto).FullName}_{nameof(CustomAddAsJson)}"))
			{
				return await SapExecute<TDto>(async (client, handler) =>
				{
					client.DefaultRequestHeaders.Accept.Clear();
					client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
					client.DefaultRequestHeaders.Add("X-Requested-With", "X");
					
					using (var request = new HttpRequestMessage(HttpMethod.Post,
						$"{EndpointUrl}{newEntity.CollectionName()}"))
					{
						var content = newEntity.ToJson();
						request.Content =
							new StringContent(content, System.Text.Encoding.UTF8, "application/json");
						await PrepareRequest(request);
						using (var response = await client.SendAsync(request))
						{
							await ApiResultHandler.EnsureSuccessfulResponse(response, isOData: false);
							TDto result = null;
							if (withResult)
							{

								var readAsStringAsync = JObject.Parse(await response.Content.ReadAsStringAsync())["d"]
									.ToString();

								try
								{
									result = readAsStringAsync.ODataJsonResultToObject<TDto>();
								}
								catch (JsonSerializationException ex)
								{
									throw new NotSupportedException(
										$"Cannot retrieve result of type {typeof(TDto).FullName}. Try Add only or AddAsOData",ex);
								}
							}

							return result;
						}
					}
				});
			}
		}

		protected override async Task<TDto> InsertEntryAsync<TDto>(TDto newEntity, bool withResult,bool autobatch)
		{
			TDto result;
			if (newEntity.AddsAsOData())
			{
				result=await base.InsertEntryAsync(newEntity, withResult, autobatch);
			}
			else
			{
				
				result = await CustomAddAsJson(newEntity,withResult);
			}

			return result;
		}
		public override async Task<string> OnResolveMetadata(CancellationToken cancellationToken = default(CancellationToken))
		{
			return await SapExecute(async (client, handler) =>
			{
				var requestUri = $"{EndpointUrl}$metadata";

				using (var request = new HttpRequestMessage(HttpMethod.Get,
					requestUri))
				{
					await PrepareRequest(request);
					using (var response = await client.SendAsync(request,cancellationToken))
					{
						response.EnsureSuccessStatusCode();
						var xml = await response.Content.ReadAsStringAsync();
						var doc = XDocument.Parse(xml);
						return doc.ToString();

					}
				}
			},cancellationToken);
		}
		}
}