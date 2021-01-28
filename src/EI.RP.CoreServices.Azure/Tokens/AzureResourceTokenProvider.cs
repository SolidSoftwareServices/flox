using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Authx;
using EI.RP.CoreServices.Azure.Configuration;
using EI.RP.CoreServices.Azure.Infrastructure.Credentials;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Http.Clients;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace EI.RP.CoreServices.Azure.Tokens
{
	class AzureResourceTokenProvider : IBearerTokenProvider
	{
		private readonly ICacheProvider _cache;
		private readonly IAzureResourceTokenSettings _settings;
		private readonly IAzureCredentialsProvider _credentialsProvider;
		private readonly IAzureGeneralSettings _azureSettings;
		private readonly IHttpClientFactory _httpClientBuilder;

		public AzureResourceTokenProvider(ICacheProvider cache, IAzureResourceTokenSettings settings, 
			IAzureCredentialsProvider credentialsProvider,IAzureGeneralSettings azureSettings,
			IHttpClientFactory httpClientBuilder)
		{
			_cache = cache;
			_settings = settings;
			_credentialsProvider = credentialsProvider;
			_azureSettings = azureSettings;
			_httpClientBuilder = httpClientBuilder;
		}

		public async Task<string> GetToken(string resource, CancellationToken cancellationToken = default(CancellationToken))
		{
			if(!_azureSettings.IsAzureEnabled) return null;
			if (resource == null)
			{
				throw new ArgumentNullException(nameof(resource));
			}
			
			return await _cache.GetOrAddAsync(resource,async()=>await GetNewToken(),maxDurationFromNow: _settings.BearerTokenCacheDuration);

			async Task<string> GetNewToken()
			{
				CancellationTokenSource cts=null;
				if (cancellationToken == default(CancellationToken))
				{
					cts=new CancellationTokenSource(TimeSpan.FromSeconds(60));
					cancellationToken = cts.Token;
				}

				try 
				{
					var connectionString = _credentialsProvider.ResolveConnectionString();
					var azureServiceTokenProvider = new AzureServiceTokenProvider(connectionString,httpClientFactory:_httpClientBuilder );
					var token = await azureServiceTokenProvider.GetAccessTokenAsync(resource, cancellationToken: cancellationToken);
					if (token == null)
					{
						throw new InvalidOperationException("Failed to retrieve JWT token");
					}
					
					return token;
				}
				finally
				{
					if (cts != null)
					{
						cts.Cancel(false);
						cts.Dispose();
					}
				}
			}
		}

		

		public async Task AppendHeaders(HttpRequestHeaders headers, string bearerTokenResource,
			CancellationToken cancellationToken = default(CancellationToken))
		{
			if (_azureSettings.IsAzureEnabled)
			{
				var getToken = GetToken(bearerTokenResource, cancellationToken);
				var apiMSubscriptionKeyAsync = _settings.ApiMSubscriptionKeyAsync();
				headers.AddOrAppendAuthorizationValue(
					$"Bearer {await getToken}");
				headers.Add("Ocp-Apim-Subscription-Key", await apiMSubscriptionKeyAsync);
			}
		}

	
	}
}
