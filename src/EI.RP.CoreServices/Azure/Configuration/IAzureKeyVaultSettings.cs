using System;

namespace EI.RP.CoreServices.Azure.Configuration
{
	public interface IAzureKeyVaultSettings:IAzureGeneralSettings
	{
		string KeyVaultUrl { get; }

		/// <summary>
		/// it forces the expiration after the cache duration
		/// </summary>
		TimeSpan KeyVaultCacheDuration { get; }

	}
}