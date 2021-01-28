using System;
using Azure.Core;
using Azure.Identity;
using EI.RP.CoreServices.Azure.Configuration;

namespace EI.RP.CoreServices.Azure.Infrastructure.Credentials.Strategies
{
	class ServicePrincipalProvider : IAzureCredentialsResolverStrategy
	{
		private readonly IAzureCredentialSettings _settings;


		private readonly Lazy<ClientSecretCredential> _credential;
		public ServicePrincipalProvider(IAzureCredentialSettings settings)
		{
			_settings = settings;
			_credential = new Lazy<ClientSecretCredential>(() => new ClientSecretCredential(
				_settings.Tenant,
				settings.ClientId,
				_settings.ClientSecret));
		}

		public CredentialType CredentialType { get; } = CredentialType.ServicePrincipal;
		public TokenCredential Build()
		{
			ThrowIfInvalid();
			return _credential.Value;
		}

		public string BuildConnectionString()
		{
			ThrowIfInvalid();
			return
				$"RunAs=App;AppId={_settings.ClientId};TenantId={_settings.Tenant};AppKey={_settings.ClientSecret}";
		}
		private void ThrowIfInvalid()
		{
			if (_settings.CredentialType != CredentialType) throw new InvalidOperationException();
		}
	}
}