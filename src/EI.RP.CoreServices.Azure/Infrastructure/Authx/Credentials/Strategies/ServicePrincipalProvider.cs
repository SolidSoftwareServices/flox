using System;
using Azure.Core;
using Azure.Identity;
using EI.RP.CoreServices.Azure.Configuration;
using EI.RP.CoreServices.DeliveryPipeline.Environments;
using Microsoft.AspNetCore.Hosting;

namespace EI.RP.CoreServices.Azure.Infrastructure.Authx.Credentials.Strategies
{
	class ServicePrincipalProvider : IAzureCredentialsResolverStrategy
	{
		private readonly IHostingEnvironment _environment;
		private  readonly Lazy<ClientSecretCredential> _credential = new Lazy<ClientSecretCredential>(() => new  ClientSecretCredential(
			"fb01cb1d-bba8-4c1a-94ef-defd79c59a09",
			"fffaa3d9-f300-4ba4-83cf-ce2b896dbfa9",
			"KimPQ_-H0yOAcwVx_P-R2hS9Wnn4xrG6-Y"));
		public ServicePrincipalProvider(IHostingEnvironment environment)
		{
			_environment = environment;
		}

		public CredentialType CredentialType { get; } = CredentialType.ServicePrincipal;
		public TokenCredential Build()
		{
			if (_environment.IsPreProduction() || _environment.IsProductionEnvironment())
			{
				throw new InvalidProgramException("This provider is not valid for this environment");
			}
			return _credential.Value;
		}
	}
}