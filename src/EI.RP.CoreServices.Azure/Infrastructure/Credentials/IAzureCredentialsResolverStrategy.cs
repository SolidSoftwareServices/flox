using Azure.Core;
using EI.RP.CoreServices.Azure.Configuration;

namespace EI.RP.CoreServices.Azure.Infrastructure.Credentials
{
	interface IAzureCredentialsResolverStrategy
	{
		CredentialType CredentialType { get; }
		TokenCredential Build();
		string BuildConnectionString();
	}
}