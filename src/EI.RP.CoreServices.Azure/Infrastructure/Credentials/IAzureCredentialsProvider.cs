using Azure.Core;

namespace EI.RP.CoreServices.Azure.Infrastructure.Credentials
{
	interface IAzureCredentialsProvider
	{
		TokenCredential Resolve();
		string ResolveConnectionString();
	}
}