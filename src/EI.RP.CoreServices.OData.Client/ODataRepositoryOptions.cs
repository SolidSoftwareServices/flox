using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.OData.Client.Infrastructure.Edmx;
using EI.RP.CoreServices.OData.Client.Infrastructure.Validation;
using EI.RP.CoreServices.Profiling;

namespace EI.RP.CoreServices.OData.Client
{
	public abstract class ODataRepositoryOptions
	{
		public IODataClientSettings ODataSettings { get; }
		public string EndpointUrl { get; }
		public IProfiler Profiler { get; }
		public double BatchEnlistTimeoutMilliseconds { get; }
		public IProxyModelValidator ModelValidator { get; }
		public IEdmxResolver EdmxResolver { get; }

		protected ODataRepositoryOptions(IODataClientSettings oDataSettings, string endpointUrl, IProfiler profiler,
			double batchEnlistTimeoutMilliseconds, IProxyModelValidator modelValidator, IEdmxResolver edmxResolver)
		{
			ODataSettings = oDataSettings;
			EndpointUrl = endpointUrl;
			Profiler = profiler;
			BatchEnlistTimeoutMilliseconds = batchEnlistTimeoutMilliseconds;
			ModelValidator = modelValidator;
			EdmxResolver = edmxResolver;
		}

		public abstract Task AppendHeaders(HttpRequestHeaders headers, CancellationToken cancellationToken = default(CancellationToken));
	}
}