namespace EI.RP.CoreServices.Azure.Configuration
{
		
	public interface IAzureCredentialSettings:IAzureGeneralSettings
	{
		CredentialType CredentialType { get; }
		string Tenant { get; }
		string ClientId { get; }
		string ClientSecret { get; }
		}
}