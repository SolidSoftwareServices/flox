using System;
using System.Threading.Tasks;

namespace EI.RP.CoreServices.Caching
{
	public interface ICacheSettings
	{
		bool IsCacheEnabled { get; }
		bool IsCachePreLoaderEnabled { get; }
		CacheProviderType CacheProviderType { get; }
		TimeSpan ExpireCacheItemsWhenNotUsedFor { get; }

		Task<string> RedisConnectionString();
	}

	public enum CacheProviderType
	{
		NoCache=0,
		InMemory ,
		Redis
		//??SqlServer
	}
}