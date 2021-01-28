using EI.RP.CoreServices.Caching;

namespace EI.RP.CoreServices.Platform
{
	public interface IPlatformSettings : ICacheSettings
	{

		bool HealthChecksEnabled { get; }

		bool ProfileInDetail { get; }

	
	}
}