using System;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Authx;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.OData.Client;
using EI.RP.CoreServices.OData.Client.Infrastructure.Edmx;
using EI.RP.CoreServices.OData.Client.Infrastructure.Validation;
using EI.RP.CoreServices.Profiling;
using EI.RP.DataServices.SAP.Clients.ErrorHandling;
using EI.RP.DataServices.SAP.Clients.Infrastructure.Session;

namespace EI.RP.DataServices.SAP.Clients.Repositories
{
	internal abstract class SapRepositoryOptions : ODataRepositoryOptions
	{
		private readonly IBearerTokenProvider _tokenProvider;
		public ISapResultStatusHandler ApiResultHandler { get; }
		public IUserSessionProvider UserSessionProvider { get; }
		public ISapSessionDataRepository SapSessionData { get; }
		public TimeSpan RequestTimeout { get; }

		protected SapRepositoryOptions(string endpointUrl,
			ISapResultStatusHandler apiResultHandler,
			IUserSessionProvider userSessionProvider,
			ISapSessionDataRepository sapSessionData, IODataClientSettings settings, IProfiler profiler,
			double sapSettingsBatchEnlistTimeoutMilliseconds, TimeSpan requestTimeout,
			IProxyModelValidator modelValidator, IEdmxResolver edmxResolver,IBearerTokenProvider tokenProvider) :base(settings, endpointUrl, profiler, sapSettingsBatchEnlistTimeoutMilliseconds, modelValidator, edmxResolver)
		{
			_tokenProvider = tokenProvider;
			ApiResultHandler = apiResultHandler;
			UserSessionProvider = userSessionProvider;
			SapSessionData = sapSessionData;
			RequestTimeout = requestTimeout;
		}



		public override async Task AppendHeaders(HttpRequestHeaders headers,
			CancellationToken cancellationToken = default(CancellationToken))
		{

			await _tokenProvider.AppendHeaders(headers, await TokenProviderUrlAsync(),
				cancellationToken);
		}

		public abstract Task<string> TokenProviderUrlAsync();
	
	}
}