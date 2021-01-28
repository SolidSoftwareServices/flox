using System;
using Azure.Core;
using Azure.Identity;
using EI.RP.CoreServices.Azure.Configuration;

namespace EI.RP.CoreServices.Azure.Infrastructure.Authx.Credentials.Strategies
{
	class DefaultProvider : IAzureCredentialsResolverStrategy
	{
		private  readonly Lazy<DefaultAzureCredential> _defaultAzureCredential = new Lazy<DefaultAzureCredential>(()=> new DefaultAzureCredential());
		public CredentialType CredentialType { get; }=CredentialType.Default;
		public TokenCredential Build()
		{
			return _defaultAzureCredential.Value;
		}
	}
}