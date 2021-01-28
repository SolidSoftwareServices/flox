using System.Threading.Tasks;

namespace EI.RP.WebApp.Infrastructure.Caching.PreLoad.Processors
{
	internal interface ICachePreloaderSettings
	{
		Task<string> CacheServiceUserNameAsync();
		Task<string> CacheServicePasswordAsync();
	}
}