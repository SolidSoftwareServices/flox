using System;
using System.Threading.Tasks;

namespace EI.RP.CoreServices.Azure.Configuration
{
	public interface IAzureResourceTokenSettings:IAzureGeneralSettings
	{
		TimeSpan BearerTokenCacheDuration { get; }
	

		Task<string> ApiMSubscriptionKeyAsync();
	}
}
