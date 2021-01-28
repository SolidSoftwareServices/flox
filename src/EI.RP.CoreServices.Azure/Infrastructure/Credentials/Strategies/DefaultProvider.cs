using System;
using Azure.Core;
using Azure.Identity;
using EI.RP.CoreServices.Azure.Configuration;

namespace EI.RP.CoreServices.Azure.Infrastructure.Credentials.Strategies
{
	class DefaultProvider : IAzureCredentialsResolverStrategy
	{
		private readonly IAzureCredentialSettings _settings;
		private  readonly Lazy<DefaultAzureCredential> _defaultAzureCredential = new Lazy<DefaultAzureCredential>(()=> new DefaultAzureCredential());

		public DefaultProvider(IAzureCredentialSettings settings)
		{
			_settings = settings;
		}

		public CredentialType CredentialType { get; }=CredentialType.Default;
		public TokenCredential Build()
		{
			ThrowIfInvalid();
			return _defaultAzureCredential.Value;
		}

		private void ThrowIfInvalid()
		{
			if (_settings.CredentialType != CredentialType) throw new InvalidOperationException();
		}

		public string BuildConnectionString()
		{
			ThrowIfInvalid();
			return null;
		}
	}
}