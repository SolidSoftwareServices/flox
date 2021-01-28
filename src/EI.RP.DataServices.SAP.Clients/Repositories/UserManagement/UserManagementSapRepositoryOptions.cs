using System.Threading;
using System.Threading.Tasks;
using EI.RP.CoreServices.Authx;
using EI.RP.CoreServices.Http.Session;
using EI.RP.CoreServices.OData.Client;
using EI.RP.CoreServices.OData.Client.Infrastructure.Edmx;
using EI.RP.CoreServices.OData.Client.Infrastructure.Validation;
using EI.RP.CoreServices.Profiling;
using EI.RP.DataServices.SAP.Clients.Config;
using EI.RP.DataServices.SAP.Clients.ErrorHandling;
using EI.RP.DataServices.SAP.Clients.Infrastructure.Session;

namespace EI.RP.DataServices.SAP.Clients.Repositories.UserManagement
{
	internal class UserManagementSapRepositoryOptions : SapRepositoryOptions
	{
		public UserManagementSapRepositoryOptions (ISapSettings sapSettings, ISapResultStatusHandler apiResultHandler,
			IUserSessionProvider userSessionProvider, ISapSessionDataRepository sapSessionDataRepository,
			IODataClientSettings oDataSettings, IProfiler profiler, IProxyModelValidator modelValidator,
			IEdmxResolver edmxResolver, IBearerTokenProvider tokenProvider) : base($"{sapSettings.SapBaseUrl}{sapSettings.UserManagementEndpoint}", apiResultHandler, userSessionProvider,
			sapSessionDataRepository, oDataSettings, profiler, sapSettings.BatchEnlistTimeoutMilliseconds, sapSettings.RequestTimeout, modelValidator, edmxResolver,tokenProvider){
			_sapSettings = sapSettings;
		}

		private readonly ISapSettings _sapSettings;

		public override async Task<string> TokenProviderUrlAsync() =>
			await _sapSettings.SapUserManagementBearerTokenProviderUrlAsync();
	}
}