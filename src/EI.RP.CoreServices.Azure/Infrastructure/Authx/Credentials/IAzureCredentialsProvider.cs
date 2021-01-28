using Azure.Core;

namespace EI.RP.CoreServices.Azure.Infrastructure.Authx.Credentials
{
	interface IAzureCredentialsProvider
	{
		TokenCredential Resolve();
	}
}