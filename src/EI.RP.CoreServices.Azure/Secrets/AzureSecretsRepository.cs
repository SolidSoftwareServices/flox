using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Security.KeyVault.Secrets;
using EI.RP.CoreServices.Azure.Configuration;
using EI.RP.CoreServices.Azure.Infrastructure.Credentials;
using EI.RP.CoreServices.Caching;
using EI.RP.CoreServices.Http.Clients;
using EI.RP.CoreServices.Profiling;
using EI.RP.CoreServices.Resiliency;
using EI.RP.CoreServices.Secrets;
using EI.RP.CoreServices.System;
using NLog;

namespace EI.RP.CoreServices.Azure.Secrets
{
	class AzureSecretsRepository : ISecretsRepository
	{

		private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
		private readonly IProfiler _profiler;
		private readonly Lazy<SecretClient> _client;
		private readonly ICacheProvider _cache;
		private readonly TimeSpan _maxDuration;
		public static readonly string InstanceId = Guid.NewGuid().ToString();
		
		public AzureSecretsRepository(IAzureKeyVaultSettings settings, ICacheProvider cache, IHttpClientBuilder httpClientBuilder, IAzureCredentialsProvider credentialsProvider,IProfiler profiler)
			:this(settings,cache, httpClientBuilder, credentialsProvider.Resolve(),profiler)
		{
		}
		internal AzureSecretsRepository(IAzureKeyVaultSettings settings, ICacheProvider cache,
			IHttpClientBuilder httpClientBuilder,
			TokenCredential credential, IProfiler profiler)
		{
			_profiler = profiler;

			_maxDuration = settings.KeyVaultCacheDuration;
			_cache = cache;
			_client = new Lazy<SecretClient>(BuildSecretClient);

			SecretClient BuildSecretClient()
			{
				
				var httpClient = httpClientBuilder.Build();
				var client = new SecretClient(new Uri(settings.KeyVaultUrl), credential,
					new SecretClientOptions() {Transport = new HttpClientTransport(httpClient)});

				return client;
			}
		}
		public async Task<string> GetAsync(string key, int maxAttempts,bool cacheResults=true, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				//IMPORTANT _cache.IsReadyToUse()==> breaks a circular deadlock secretsrepo-->...--> encryptionsecrets--->secretsrepo
				var result = cacheResults &&_cache.IsReadyToUse()
					?  await _cache.GetOrAddAsync(key, async()=>await ExecuteGet(),InstanceId,maxDurationFromNow: _maxDuration,cancellationToken:cancellationToken)
					:await ExecuteGet();
				Logger.Debug(()=>$"retrieved secret with key {key} and value {result??"(null)"}. Cache duration:{_maxDuration}");
				
				return result;
			}
			catch (RequestFailedException ex)
			{
				return WhenRequestFailed(key, ex);
			}

			async Task<string> ExecuteGet()
			{

				Logger.Info(() => $"Fetching key {key} from keyvault.");
				using (_profiler.RecordStep($"{nameof(AzureSecretsRepository)}.{nameof(GetAsync)}({key})"))
				{
					CancellationTokenSource cts = null;
					if (cancellationToken == CancellationToken.None)
					{
						cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
						cancellationToken = cts.Token;
					}

					try
					{
						var response =
							await ResilientOperations.Default.RetryIfNeeded(
								async () =>
								{

									var secretAsync = await _client.Value.GetSecretAsync(key,
										cancellationToken: cancellationToken);
									if (secretAsync == null) throw new NullReferenceException();
									return secretAsync;
								}, cancellationToken,
								maxAttempts);
						Logger.Info(() => $"Completed Fetching key {key} from keyvault.");
						var processGetResult = _ProcessGetResult(key, response);

						return processGetResult;
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
		}

		
		


		public async Task SetAsync(string key, string value)
		{
			await Task.WhenAll(
				_client.Value.SetSecretAsync(new KeyVaultSecret(key, value)),
				_cache.InvalidateAsync(InstanceId,default(CancellationToken), key)
			);
		}



		public async Task RemoveAsync(string key)
		{
			try
			{
				await _client.Value.StartDeleteSecretAsync(key);

			}
			catch (RequestFailedException ex)
			{
				WhenRequestFailed(key, ex);
			}
			finally
			{
				await _cache.InvalidateAsync(InstanceId,default(CancellationToken), key);
			}

		}

		private static string WhenRequestFailed(string key, RequestFailedException ex)
		{
			if (ex.Status == 404) throw new KeyNotFoundException($"{key} not found found", ex);
			throw ex;
		}



		private static string _ProcessGetResult(string key, Response<KeyVaultSecret> response)
		{
			if (response == null
			    || response.Value == null
			    || response.Value.Properties.Enabled.HasValue && !response.Value.Properties.Enabled.Value)
			{
				throw new KeyNotFoundException(key);
			}

			return response.Value.Value;
		}

	

		public async Task<bool> ContainsAsync(string key)
		{
			try
			{
				await GetAsync(key, 3);
				return true;
			}
			catch (KeyNotFoundException)
			{
				return false;
			}
		}
	}
}