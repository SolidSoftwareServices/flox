using System.Collections.Generic;
using System.Linq;
using Azure.Core;
using EI.RP.CoreServices.Azure.Configuration;

namespace EI.RP.CoreServices.Azure.Infrastructure.Credentials
{
	class AzureCredentialsProvider : IAzureCredentialsProvider
	{
		private readonly IAzureCredentialsResolverStrategy _strategy;
		

		public AzureCredentialsProvider(IAzureCredentialSettings settings,IEnumerable<IAzureCredentialsResolverStrategy> strategies)
		{
			_strategy=strategies.Single(x => x.CredentialType == settings.CredentialType);
		}

		public TokenCredential Resolve()
		{
			return _strategy.Build();
		}

		public string ResolveConnectionString()
		{

			return _strategy.BuildConnectionString();
		}
	}
}
