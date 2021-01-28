namespace EI.HybridAdapter.WebApi.Infrastructure.Settings
{
	public interface IAzureSettings
	{
		string JwtAudience { get; }
		string AzureAdInstance { get; }
		string AzureAdTenant { get; }
	}
}